namespace App.Models
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class DatabaseValidationSettings
    {
        private readonly IDictionary<string, Regex> _dataTypes;

        public DatabaseValidationSettings()
        {
            // TODO: Load settings from config file.
            this._dataTypes = new Dictionary<string, Regex>
            {
                { "char", new Regex(@"") }, { "integer", new Regex(@"") }, { "real", new Regex(@"") },
                { "complexInteger", new Regex(@"") }, { "complexReal", new Regex(@"") }
            };
        }

        public DatabaseValidationSettings(IDictionary<string, Regex> dataTypes)
        {
            this._dataTypes = dataTypes;
        }

        public IDictionary<string, Regex> DataTypes => this._dataTypes;
    }
}