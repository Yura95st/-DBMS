namespace App.Models.DataTypes.Concrete
{
    using App.Models.DataTypes.Abstract;

    public class IntDataType : IDataType
    {
        #region IDataType Members

        public bool IsValidValue(string value)
        {
            int x;
            return int.TryParse(value, out x);
        }

        public string DefaultValue => "0";

        #endregion
    }
}