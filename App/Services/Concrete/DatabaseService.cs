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
            Guard.NotNull(dbName, "dbName");

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
            Database db = this.GetDatabase(dbName);
            if (db == null)
            {
                throw new DatabaseNotFoundException($"Database with name \"{dbName}\" does not exist.");
            }

            Directory.Delete(this.GetDatabasePath(db.Name), true);
        }

        public void DropTable(string dbName, string tableName)
        {
            throw new NotImplementedException();
        }

        public Database GetDatabase(string dbName)
        {
            Guard.NotNull(dbName, "dbName");

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
            throw new NotImplementedException();
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