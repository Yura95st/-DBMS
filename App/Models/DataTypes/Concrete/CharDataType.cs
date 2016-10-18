namespace App.Models.DataTypes.Concrete
{
    using App.Models.DataTypes.Abstract;

    public class CharDataType : IDataType
    {
        #region IDataType Members

        public bool IsValidValue(string value)
        {
            char x;
            return char.TryParse(value, out x);
        }

        public string DefaultValue => "";

        #endregion
    }
}