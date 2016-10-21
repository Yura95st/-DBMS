namespace Domain.Models.DataTypes.Concrete
{
    public class ComplexIntDataType : ComplexDataType
    {
        public ComplexIntDataType()
            : base(@"\d")
        {
        }
    }
}