namespace App.Services.Concrete
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using App.Dal.Repositories.Abstract;
    using App.Exceptions;
    using App.Models;
    using App.Services.Abstract;
    using App.Utils;
    using App.Validations.Abstract;

    public class DatabaseService : IDatabaseService
    {
        private readonly IDatabaseValidation _databaseValidation;

        private readonly IDbRepository _repository;

        public DatabaseService(IDbRepository repository, IDatabaseValidation databaseValidation)
        {
            Guard.NotNull(repository, "repository");
            Guard.NotNull(databaseValidation, "databaseValidation");

            this._repository = repository;
            this._databaseValidation = databaseValidation;
        }

        #region IDatabaseService Members

        public void AddRow(string dbName, string tableName, Row row)
        {
            Guard.NotNullOrEmpty(dbName, "dbName");
            Guard.NotNullOrEmpty(tableName, "tableName");
            Guard.NotNull(row, "row");

            Database db = this.GetDatabase(dbName);
            if (db == null)
            {
                throw new DatabaseNotFoundException($"Database with name \"{dbName}\" does not exist.");
            }

            Table table;
            db.Tables.TryGetValue(tableName, out table);
            if (table == null)
            {
                throw new TableNotFoundException($"Table with name \"{tableName}\" does not exist in database \"{dbName}\".");
            }

            if (!this._databaseValidation.DoesRowFitTable(table, row))
            {
                throw new InvalidRowException("Row does not fit table.");
            }

            row.Id = table.NextRowId;
            table.Rows.Add(row.Id, row);

            table.NextRowId++;

            this.UpdateDatabase(db);
        }

        public void CreateDatabase(string dbName)
        {
            Guard.NotNullOrEmpty(dbName, "dbName");

            if (!this._databaseValidation.IsValidDatabaseName(dbName))
            {
                throw new InvalidDatabaseNameException($"Database name \"{dbName}\" has invalid format.");
            }

            IEnumerable<string> dbNames = this.GetDatabaseNames();
            if (dbNames.Contains(dbName))
            {
                throw new DatabaseAlreadyExistsException($"Database with name \"{dbName}\" already exists.");
            }

            Database database = new Database { Name = dbName };
            try
            {
                this._repository.Create(database);
            }
            catch (DbRepositoryException ex)
            {
                throw new DbServiceException("Error occurred while creating database. See inner exception for details.", ex);
            }
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

            if (db.Tables.ContainsKey(tableScheme.Name))
            {
                throw new TableAlreadyExistsException(
                    $"Table with name \"{tableScheme.Name}\" already exists in database \"{dbName}\".");
            }

            Table table = new Table { Name = tableScheme.Name, Attributes = tableScheme.Attributes.ToList() };

            db.Tables.Add(table.Name, table);

            this.UpdateDatabase(db);
        }

        public void DeleteRow(string dbName, string tableName, int rowId)
        {
            Guard.NotNullOrEmpty(dbName, "dbName");
            Guard.NotNullOrEmpty(tableName, "tableName");
            Guard.IntMoreOrEqualToZero(rowId, "rowId");

            Database db = this.GetDatabase(dbName);
            if (db == null)
            {
                throw new DatabaseNotFoundException($"Database with name \"{dbName}\" does not exist.");
            }

            Table table;
            db.Tables.TryGetValue(tableName, out table);
            if (table == null)
            {
                throw new TableNotFoundException($"Table with name \"{tableName}\" does not exist in database \"{dbName}\".");
            }

            if (!table.Rows.ContainsKey(rowId))
            {
                throw new RowNotFoundException($"Row with id \"{rowId}\" does not exist in table \"{tableName}\".");
            }

            table.Rows.Remove(rowId);

            this.UpdateDatabase(db);
        }

        public void DropDatabase(string dbName)
        {
            Guard.NotNullOrEmpty(dbName, "dbName");

            IEnumerable<string> dbNames = this.GetDatabaseNames();
            if (!dbNames.Contains(dbName))
            {
                throw new DatabaseNotFoundException($"Database with name \"{dbName}\" does not exist.");
            }

            try
            {
                this._repository.Delete(dbName);
            }
            catch (DbRepositoryException ex)
            {
                throw new DbServiceException("Error occurred while deleting database. See inner exception for details.", ex);
            }
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
            if (!db.Tables.ContainsKey(tableName))
            {
                throw new TableNotFoundException($"Table with name \"{tableName}\" does not exist in database \"{dbName}\".");
            }

            db.Tables.Remove(tableName);

            this.UpdateDatabase(db);
        }

        public Database GetDatabase(string dbName)
        {
            Guard.NotNullOrEmpty(dbName, "dbName");

            try
            {
                Database db = this._repository.GetByName(dbName);
                return db;
            }
            catch (DbRepositoryException ex)
            {
                throw new DbServiceException(
                    "Error occurred while getting all databases names. See inner exception for details.", ex);
            }
        }

        public IEnumerable<string> GetDatabaseNames()
        {
            try
            {
                IEnumerable<string> dbNames = this._repository.GetAllNames();
                return dbNames;
            }
            catch (DbRepositoryException ex)
            {
                throw new DbServiceException(
                    "Error occurred while getting all databases names. See inner exception for details.", ex);
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

            Table table;
            db.Tables.TryGetValue(tableName, out table);

            return table;
        }

        public Table GetTableProjection(string dbName, string tableName, IEnumerable<string> attributesNames)
        {
            Guard.NotNullOrEmpty(dbName, "dbName");
            Guard.NotNullOrEmpty(tableName, "tableName");
            Guard.NotNullOrEmpty(attributesNames, "attributesNames");

            Table table = this.GetTable(dbName, tableName);
            if (table != null)
            {
                HashSet<int> attributesIndexes = new HashSet<int>();

                foreach (string attributeName in attributesNames)
                {
                    Models.Attribute attribute = table.Attributes.FirstOrDefault(a => a.Name == attributeName);
                    if (attribute == null)
                    {
                        throw new AttributeNotFoundException(
                            $"Attribute with name \"{attributeName}\" does not exist in table \"{tableName}\"");
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
            }

            return table;
        }

        public void UpdateRow(string dbName, string tableName, Row row)
        {
            Guard.NotNullOrEmpty(dbName, "dbName");
            Guard.NotNullOrEmpty(tableName, "tableName");
            Guard.NotNull(row, "row");

            Database db = this.GetDatabase(dbName);
            if (db == null)
            {
                throw new DatabaseNotFoundException($"Database with name \"{dbName}\" does not exist.");
            }

            Table table;
            db.Tables.TryGetValue(tableName, out table);
            if (table == null)
            {
                throw new TableNotFoundException($"Table with name \"{tableName}\" does not exist in database \"{dbName}\".");
            }

            if (!table.Rows.ContainsKey(row.Id))
            {
                throw new RowNotFoundException($"Row with id \"{row.Id}\" does not exist in table \"{tableName}\".");
            }

            if (!this._databaseValidation.DoesRowFitTable(table, row))
            {
                throw new InvalidRowException("Row does not fit table.");
            }

            table.Rows[row.Id] = row;

            this.UpdateDatabase(db);
        }

        #endregion

        private void UpdateDatabase(Database db)
        {
            try
            {
                this._repository.Update(db);
            }
            catch (DbRepositoryException ex)
            {
                throw new DbServiceException("DbRepository error occurred. See inner exception for details.", ex);
            }
        }
    }
}