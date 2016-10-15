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

        public void AddRow(string dbName, string tableName, Row row)
        {
            Guard.NotNullOrEmpty(dbName, "dbName");
            Guard.NotNullOrEmpty(tableName, "tableName");
            Guard.NotNull(row, "row");

            IEnumerable<string> dbNames = this.GetDatabaseNames();
            if (!dbNames.Contains(dbName))
            {
                throw new DatabaseNotFoundException($"Database with name \"{dbName}\" does not exist.");
            }

            Table table = this.GetTable(dbName, tableName);
            if (table == null)
            {
                throw new TableNotFoundException($"Table with name \"{tableName}\" does not exist in database \"{dbName}\".");
            }

            if (!this._databaseValidation.DoesRowFitTable(table, row))
            {
                throw new InvalidRowException("Row is invalid. See inner exception for details.");
            }

            row.Id = table.NextRowId;
            table.Rows.Add(row.Id, row);

            table.NextRowId++;

            this.WriteTableToFile(table, dbName);
        }

        public void CreateDatabase(string dbName)
        {
            Guard.NotNullOrEmpty(dbName, "dbName");

            if (!this._databaseValidation.IsValidDatabaseName(dbName))
            {
                throw new InvalidNameFormatException($"Database name \"{dbName}\" has invalid format.");
            }

            IEnumerable<string> dbNames = this.GetDatabaseNames();
            if (dbNames.Contains(dbName))
            {
                throw new DatabaseAlreadyExistsException($"Database with name \"{dbName}\" already exists.");
            }

            Directory.CreateDirectory(this.GetDatabasePath(dbName));
        }

        public void CreateTable(string dbName, TableScheme tableScheme)
        {
            Guard.NotNullOrEmpty(dbName, "dbName");
            Guard.NotNull(tableScheme, "tableScheme");

            Database db = this.GetDatabase(dbName);
            if (db == null)
            {
                throw new DatabaseNotFoundException($"Database with name \"{dbName}\" does not exist.");
            }

            try
            {
                this._databaseValidation.CheckTableScheme(tableScheme);
            }
            catch (Exception ex)
            {
                throw new InvalidTableSchemeException("Table scheme is invalid. See inner exception for details.", ex);
            }

            if (db.TableNames.Contains(tableScheme.Name))
            {
                throw new TableAlreadyExistsException(
                    $"Table with name \"{tableScheme.Name}\" already exists in database \"{dbName}\".");
            }

            string tablesDirectoryPath = this.GetTablesDirectoryPath(dbName);
            Directory.CreateDirectory(tablesDirectoryPath);

            Table table = new Table { Name = tableScheme.Name, Attributes = tableScheme.Attributes.ToList() };

            this.WriteTableToFile(table, dbName);
        }

        public void DeleteRow(string dbName, string tableName, int rowId)
        {
            Guard.NotNullOrEmpty(dbName, "dbName");
            Guard.NotNullOrEmpty(tableName, "tableName");
            Guard.IntMoreOrEqualToZero(rowId, "rowId");

            IEnumerable<string> dbNames = this.GetDatabaseNames();
            if (!dbNames.Contains(dbName))
            {
                throw new DatabaseNotFoundException($"Database with name \"{dbName}\" does not exist.");
            }

            Table table = this.GetTable(dbName, tableName);
            if (table == null)
            {
                throw new TableNotFoundException($"Table with name \"{tableName}\" does not exist in database \"{dbName}\".");
            }

            if (!table.Rows.ContainsKey(rowId))
            {
                throw new RowNotFoundException($"Row with id \"{rowId}\" does not exist in table \"{tableName}\".");
            }

            table.Rows.Remove(rowId);

            this.WriteTableToFile(table, dbName);
        }

        public void DropDatabase(string dbName)
        {
            Guard.NotNullOrEmpty(dbName, "dbName");

            IEnumerable<string> dbNames = this.GetDatabaseNames();
            if (!dbNames.Contains(dbName))
            {
                throw new DatabaseNotFoundException($"Database with name \"{dbName}\" does not exist.");
            }

            Directory.Delete(this.GetDatabasePath(dbName), true);
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

        public IEnumerable<string> GetDatabaseNames()
        {
            if (Directory.Exists(this._settings.StoragePath))
            {
                foreach (string dirName in Directory.EnumerateDirectories(this._settings.StoragePath))
                {
                    yield return Path.GetFileName(dirName);
                }
            }
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

        public Table GetTableProjection(string dbName, string tableName, IEnumerable<string> attributesNames)
        {
            Guard.NotNullOrEmpty(dbName, "dbName");
            Guard.NotNullOrEmpty(tableName, "tableName");
            Guard.NotNullOrEmpty(attributesNames, "attributesNames");

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

            Table table;
            try
            {
                table = JsonConvert.DeserializeObject<Table>(tableJson);
            }
            catch (JsonException)
            {
                return null;
            }

            HashSet<int> attributesIndexes = new HashSet<int>();

            foreach (string attributeName in attributesNames)
            {
                Models.Attribute attribute = table.Attributes.FirstOrDefault(a => a.Name == attributeName);
                if (attribute == null)
                {
                    throw new AttributeNotFoundException($"Attribute with name \"{attributeName}\" does not exist in table \"{tableName}\"");
                }

                attributesIndexes.Add(table.Attributes.IndexOf(attribute));
            }

            for (int i = 0; i < table.Attributes.Count; i++)
            {
                if (!attributesIndexes.Contains(i))
                {
                    table.Attributes.RemoveAt(i);

                    foreach (Row row in table.Rows.Values)
                    {
                        row.Value.RemoveAt(i);
                    }
                }
            }

            return table;
        }

        public void UpdateRow(string dbName, string tableName, Row row)
        {
            Guard.NotNullOrEmpty(dbName, "dbName");
            Guard.NotNullOrEmpty(tableName, "tableName");
            Guard.NotNull(row, "row");

            IEnumerable<string> dbNames = this.GetDatabaseNames();
            if (!dbNames.Contains(dbName))
            {
                throw new DatabaseNotFoundException($"Database with name \"{dbName}\" does not exist.");
            }

            Table table = this.GetTable(dbName, tableName);
            if (table == null)
            {
                throw new TableNotFoundException($"Table with name \"{tableName}\" does not exist in database \"{dbName}\".");
            }

            if (!table.Rows.ContainsKey(row.Id))
            {
                throw new RowNotFoundException($"Row with id \"{row.Id}\" does not exist in table \"{tableName}\".");
            }

            try
            {
                this._databaseValidation.DoesRowFitTable(table, row);
            }
            catch (Exception ex)
            {
                throw new InvalidRowException("Row is invalid. See inner exception for details.", ex);
            }

            table.Rows[row.Id] = row;

            this.WriteTableToFile(table, dbName);
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
            return Path.Combine(this.GetDatabasePath(dbName), $"{this._settings.TablesDirectoryName}/");
        }

        private void WriteTableToFile(Table table, string dbName)
        {
            string tablePath = this.GetTablePath(dbName, table.Name);
            string tableJson = JsonConvert.SerializeObject(table);

            File.WriteAllText(tablePath, tableJson);
        }
    }
}