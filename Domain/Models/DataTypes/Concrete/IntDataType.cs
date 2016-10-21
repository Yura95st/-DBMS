namespace Domain.Models.DataTypes.Concrete
{
    using Domain.Models.DataTypes.Abstract;

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