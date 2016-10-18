namespace App.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using App.Models.DataTypes.Abstract;
    using App.Models.DataTypes.Concrete;

    public class DatabaseValidationSettings
    {
        private readonly IDictionary<string, IDataType> _dataTypes;

        public DatabaseValidationSettings()
        {
            // TODO: Load settings from config file.
            this._dataTypes = new Dictionary<string, IDataType>
            {
                { "char", new CharDataType() }, { "integer", new IntDataType() }, { "double", new DoubleDataType() },
                { "complexDouble", new ComplexDoubleDataType() }, { "complexInteger", new ComplexIntDataType() }
            };
        }

        public DatabaseValidationSettings(IDictionary<string, IDataType> dataTypes)
        {
            this._dataTypes = dataTypes;
        }

        public IDictionary<string, IDataType> DataTypes => this._dataTypes;
    }
}