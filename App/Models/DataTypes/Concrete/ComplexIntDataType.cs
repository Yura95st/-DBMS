namespace App.Models.DataTypes.Concrete
{
    using System;

    using App.Models.DataTypes.Abstract;

    public class ComplexIntDataType : IDataType
    {
        #region IDataType Members

        public bool IsValidValue(string value)
        {
            throw new NotImplementedException();
        }

        public string DefaultValue
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}