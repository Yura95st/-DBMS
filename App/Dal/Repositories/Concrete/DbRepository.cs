namespace App.Dal.Repositories.Concrete
{
    using System.Collections.Generic;
    using System.IO;

    using App.Dal.Repositories.Abstract;
    using App.Exceptions;
    using App.Models;
    using App.Utils;

    using Newtonsoft.Json;

    public class DbRepository : IDbRepository
    {
        private readonly DbRepositorySettings _settings;

        public DbRepository(DbRepositorySettings settings)
        {
            Guard.NotNull(settings, "settings");

            this._settings = settings;
        }

        #region IDbRepository Members

        public void Create(Database database)
        {
            Guard.NotNull(database, "database");

            if (File.Exists(this.GetDbFilePath(database.Name)))
            {
                throw new DbRepositoryException("Database with such name already exists.");
            }

            string databasesDirectoryPath = this.GetDatabasesDirectoryPath();
            try
            {
                Directory.CreateDirectory(databasesDirectoryPath);

                this.WriteDatabaseToFile(database);
            }
            catch (IOException ex)
            {
                throw new DbRepositoryException(
                    "Error occurred while creating a database. See inner exception for details.", ex);
            }
        }

        public void Delete(string dbName)
        {
            Guard.NotNullOrEmpty(dbName, "dbName");

            string dbFilePath = this.GetDbFilePath(dbName);
            if (!File.Exists(dbFilePath))
            {
                return;
            }

            try
            {
                File.Delete(dbFilePath);
            }
            catch (IOException ex)
            {
                throw new DbRepositoryException(
                    "Error occurred while deleting a database. See inner exception for details.", ex);
            }
        }

        public IEnumerable<string> GetAllNames()
        {
            string databasesDirectoryPath = this.GetDatabasesDirectoryPath();
            if (!Directory.Exists(databasesDirectoryPath))
            {
                yield break;
            }

            foreach (string fileName in Directory.EnumerateFiles(databasesDirectoryPath))
            {
                yield return Path.GetFileNameWithoutExtension(fileName);
            }
        }

        public Database GetByName(string dbName)
        {
            Guard.NotNullOrEmpty(dbName, "dbName");

            string dbFilePath = this.GetDbFilePath(dbName);
            if (!File.Exists(dbFilePath))
            {
                return null;
            }

            try
            {
                string dbJson = File.ReadAllText(dbFilePath);
                Database database = JsonConvert.DeserializeObject<Database>(dbJson);

                return database;
            }
            catch (IOException ex)
            {
                throw new DbRepositoryException(
                    "Error occurred while reading database file. See inner exception for details.", ex);
            }
            catch (JsonException ex)
            {
                throw new DbRepositoryException(
                    "Error occurred while parsing database file. See inner exception for details.", ex);
            }
        }

        public void Update(Database database)
        {
            Guard.NotNull(database, "database");

            string dbFilePath = this.GetDbFilePath(database.Name);
            if (!File.Exists(dbFilePath))
            {
                throw new DbRepositoryException("Database with such does not exist.");
            }

            try
            {
                this.WriteDatabaseToFile(database);
            }
            catch (IOException ex)
            {
                throw new DbRepositoryException(
                    "Error occurred while updating a database. See inner exception for details.", ex);
            }
        }

        #endregion

        private string GetDatabasesDirectoryPath()
        {
            return Path.Combine(this._settings.StoragePath, this._settings.DatabasesDirectoryName);
        }

        private string GetDbFilePath(string dbName)
        {
            string dbFileName = string.Format(this._settings.DbFileNameFormat, dbName);

            return Path.Combine(this.GetDatabasesDirectoryPath(), dbFileName);
        }

        private void WriteDatabaseToFile(Database database)
        {
            string dbFilePath = this.GetDbFilePath(database.Name);
            string dbJson = JsonConvert.SerializeObject(database);

            File.WriteAllText(dbFilePath, dbJson);
        }
    }
}