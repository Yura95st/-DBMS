namespace Domain.Models.DataTypes.Concrete
{
    using Domain.Models.DataTypes.Abstract;

    public class DoubleDataType : IDataType
    {
        #region IDataType Members

        public bool IsValidValue(string value)
        {
            double x;
            return double.TryParse(value, out x);
        }

        public string DefaultValue => "0.0";

        #endregion
    }
}