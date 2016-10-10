namespace App.Models
{
    public class DatabaseServiceSettings
    {
        private readonly string _storagePath;

        private readonly string _tableFileNameFormat;

        private readonly string _tablesDirectoryName;

        public DatabaseServiceSettings()
        {
            // TODO: Load settings from config file.
            this._storagePath = "path_to_storage/databases/";
            this._tableFileNameFormat = "{0}.json";
            this._tablesDirectoryName = "tables";
        }

        public DatabaseServiceSettings(string storagePath, string tableFileNameFormat, string tablesDirectoryName)
        {
            this._storagePath = storagePath;
            this._tableFileNameFormat = tableFileNameFormat;
            this._tablesDirectoryName = tablesDirectoryName;
        }

        public string StoragePath => this._storagePath;

        public string TableFileNameFormat => this._tableFileNameFormat;

        public string TablesDirectoryName => this._tablesDirectoryName;
    }
}