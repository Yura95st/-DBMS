namespace App.Models
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class DatabaseServiceSettings
    {
        public IDictionary<string, Regex> DataTypes
        {
            get;
            set;
        }

        public string StoragePath
        {
            get;
            set;
        }

        public string TablesDirectoryName
        {
            get;
            set;
        }

        public string TableFileNameFormat
        {
            get;
            set;
        }
    }
}