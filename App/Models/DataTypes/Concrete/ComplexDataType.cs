namespace App.Models.DataTypes.Concrete
{
    using System.Text.RegularExpressions;

    using App.Models.DataTypes.Abstract;

    public abstract class ComplexDataType : IDataType
    {
        protected readonly Regex _regex;

        protected readonly string _regexString = "^[+|-]?(({0})*[+|-])?({0})*[i]$";

        protected ComplexDataType(string numbersTypeRegexString)
        {
            this._regex = new Regex(string.Format(this._regexString, numbersTypeRegexString));
        }

        #region IDataType Members

        public bool IsValidValue(string value)
        {
            return value != null && this._regex.IsMatch(value);
        }

        public string DefaultValue => "i";

        #endregion
    }
}