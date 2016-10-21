namespace Domain.Models.DataTypes.Concrete
{
    using Domain.Models.DataTypes.Abstract;

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