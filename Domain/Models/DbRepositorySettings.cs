namespace Domain.Models
{
    using System;

    public class DbRepositorySettings
    {
        private readonly string _databasesDirectoryName;

        private readonly string _dbFileNameFormat;

        private readonly string _storagePath;

        public DbRepositorySettings()
        {
            // TODO: Load settings from config file.
            this._storagePath = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            this._dbFileNameFormat = "{0}.db";
            this._databasesDirectoryName = "databases";
        }

        public DbRepositorySettings(string storagePath, string dbFileNameFormat, string databasesDirectoryName)
        {
            this._storagePath = storagePath;
            this._dbFileNameFormat = dbFileNameFormat;
            this._databasesDirectoryName = databasesDirectoryName;
        }

        public string DatabasesDirectoryName => this._databasesDirectoryName;

        public string DbFileNameFormat => this._dbFileNameFormat;

        public string StoragePath => this._storagePath;
    }
}