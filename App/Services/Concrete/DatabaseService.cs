namespace App.Services.Concrete
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using App.Exceptions;
    using App.Models;
    using App.Services.Abstract;
    using App.Utils;
    using App.Validations.Abstract;

    using Newtonsoft.Json;

    public class DatabaseService : IDatabaseService
    {
        private readonly IDatabaseValidation _databaseValidation;

        private readonly DatabaseServiceSettings _settings;

        public DatabaseService(DatabaseServiceSettings settings, IDatabaseValidation databaseValidation)
        {
            Guard.NotNull(settings, "settings");
            Guard.NotNull(databaseValidation, "databaseValidation");

            this._settings = settings;
            this._databaseValidation = databaseValidation;
        }

        #region IDatabaseService Members

        public void CreateDatabase(string dbName)
        {
            Guard.NotNullOrEmpty(dbName, "dbName");

            if (!this._databaseValidation.IsValidDatabaseName(dbName))
            {
                throw new InvalidNameFormatException($"Database name \"{dbName}\" has invalid format.");
            }

            if (this.GetDatabase(dbName) != null)
            {
                throw new DatabaseAlreadyExistsException($"Database with name \"{dbName}\" already exists.");
            }

            Directory.CreateDirectory(this.GetDatabasePath(dbName));
        }

        public void CreateTable(string dbName, Table table)
        {
            Guard.NotNullOrEmpty(dbName, "dbName");
            Guard.NotNull(table, "table");

            Database db = this.GetDatabase(dbName);
            if (db == null)
            {
                throw new DatabaseNotFoundException($"Database with name \"{dbName}\" does not exist.");
            }

            try
            {
                this._databaseValidation.CheckTable(table);
            }
            catch (Exception ex)
            {
                throw new InvalidTableException("Table is invalid. See inner exception for details.", ex);
            }

            if (db.TableNames.Contains(table.Name))
            {
                throw new TableAlreadyExistsException(
                    $"Table with name \"{table.Name}\" already exists in database \"{dbName}\".");
            }

            string tablesDirectoryPath = this.GetTablesDirectoryPath(dbName);
            Directory.CreateDirectory(tablesDirectoryPath);

            string tablePath = this.GetTablePath(dbName, table.Name);
            string tableJson = JsonConvert.SerializeObject(table);

            File.WriteAllText(tablePath, tableJson);
        }

        public void DropDatabase(string dbName)
        {
            Guard.NotNullOrEmpty(dbName, "dbName");

            Database db = this.GetDatabase(dbName);
            if (db == null)
            {
                throw new DatabaseNotFoundException($"Database with name \"{dbName}\" does not exist.");
            }

            Directory.Delete(this.GetDatabasePath(db.Name), true);
        }

        public void DropTable(string dbName, string tableName)
        {
            Guard.NotNullOrEmpty(dbName, "dbName");
            Guard.NotNullOrEmpty(tableName, "tableName");

            Database db = this.GetDatabase(dbName);
            if (db == null)
            {
                throw new DatabaseNotFoundException($"Database with name \"{dbName}\" does not exist.");
            }

            if (!db.TableNames.Contains(tableName))
            {
                throw new TableNotFoundException($"Table with name \"{tableName}\" does not exist in database \"{dbName}\".");
            }

            string tablePath = this.GetTablePath(dbName, tableName);
            File.Delete(tablePath);
        }

        public Database GetDatabase(string dbName)
        {
            Guard.NotNullOrEmpty(dbName, "dbName");

            string dbPath = this.GetDatabasePath(dbName);
            if (!Directory.Exists(dbPath))
            {
                return null;
            }

            Database db = new Database { Name = dbName };

            string tablesPath = this.GetTablesDirectoryPath(dbName);
            if (Directory.Exists(tablesPath))
            {
                List<string> tableNames = new List<string>();
                foreach (string fileName in Directory.EnumerateFiles(tablesPath))
                {
                    tableNames.Add(Path.GetFileNameWithoutExtension(fileName));
                }

                db.TableNames = tableNames;
            }

            return db;
        }

        public Table GetTable(string dbName, string tableName)
        {
            Guard.NotNullOrEmpty(dbName, "dbName");
            Guard.NotNullOrEmpty(tableName, "tableName");

            Database db = this.GetDatabase(dbName);
            if (db == null)
            {
                throw new DatabaseNotFoundException($"Database with name \"{dbName}\" does not exist.");
            }

            if (!db.TableNames.Contains(tableName))
            {
                return null;
            }

            string tablePath = this.GetTablePath(dbName, tableName);
            string tableJson = File.ReadAllText(tablePath);

            try
            {
                Table table = JsonConvert.DeserializeObject<Table>(tableJson);

                return table;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        #endregion

        private string GetDatabasePath(string dbName)
        {
            return Path.Combine(this._settings.StoragePath, dbName);
        }

        private string GetTablePath(string dbName, string tableName)
        {
            return Path.Combine(this.GetTablesDirectoryPath(dbName),
                string.Format(this._settings.TableFileNameFormat, tableName));
        }

        private string GetTablesDirectoryPath(string dbName)
        {
            return Path.Combine(this.GetDatabasePath(dbName), $@"{this._settings.TablesDirectoryName}\");
        }
    }
}