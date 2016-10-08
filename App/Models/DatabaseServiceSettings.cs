namespace App.Models
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class DatabaseServiceSettings
    {
        private readonly IDictionary<string, Regex> _dataTypes;

        private readonly string _storagePath;

        private readonly string _tableFileNameFormat;

        private readonly string _tablesDirectoryName;

        public DatabaseServiceSettings()
        {
            // TODO: Load settings from config file.
            this._dataTypes = new Dictionary<string, Regex>
            {
                { "char", new Regex(@"") }, { "integer", new Regex(@"") }, { "real", new Regex(@"") },
                { "complexInteger", new Regex(@"") }, { "complexReal", new Regex(@"") }
            };
            this._storagePath = "path_to_storage/databases/";
            this._tableFileNameFormat = "{0}.json";
            this._tablesDirectoryName = "tables";
        }

        public DatabaseServiceSettings(IDictionary<string, Regex> dataTypes, string storagePath, string tableFileNameFormat,
                                       string tablesDirectoryName)
        {
            this._dataTypes = dataTypes;
            this._storagePath = storagePath;
            this._tableFileNameFormat = tableFileNameFormat;
            this._tablesDirectoryName = tablesDirectoryName;
        }

        public IDictionary<string, Regex> DataTypes => this._dataTypes;

        public string StoragePath => this._storagePath;

        public string TableFileNameFormat => this._tableFileNameFormat;

        public string TablesDirectoryName => this._tablesDirectoryName;
    }
}