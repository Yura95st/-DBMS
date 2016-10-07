namespace App.Services.Concrete
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using App.Exceptions;
    using App.Models;
    using App.Services.Abstract;
    using App.Utils;
    using App.Validations.Abstract;

    public class DatabaseService : IDatabaseService
    {
        public static readonly string TablesDirectoryName = "tables";

        private readonly INameValidation _nameValidation;

        private readonly DatabaseServiceSettings _settings;

        public DatabaseService(DatabaseServiceSettings settings, INameValidation nameValidation)
        {
            Guard.NotNull(settings, "settings");
            Guard.NotNull(nameValidation, "nameValidation");

            this._settings = settings;
            this._nameValidation = nameValidation;
        }

        #region IDatabaseService Members

        public void CreateDatabase(string dbName)
        {
            Guard.NotNull(dbName, "dbName");

            if (!this._nameValidation.IsValidDatabaseName(dbName))
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
            throw new NotImplementedException();
        }

        public void DropDatabase(string dbName)
        {
            throw new NotImplementedException();
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

            string tablesPath = this.GetTablesDirectoryPath(dbPath);
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

        public void UpdateTable(string dbName, Table table)
        {
            throw new NotImplementedException();
        }

        #endregion

        private string GetDatabasePath(string dbName)
        {
            return Path.Combine(this._settings.StoragePath, dbName);
        }

        private string GetTablesDirectoryPath(string dbPath)
        {
            return Path.Combine(dbPath, $@"{DatabaseService.TablesDirectoryName}\");
        }
    }
}