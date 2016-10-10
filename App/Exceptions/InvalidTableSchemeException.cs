namespace App.Exceptions
{
    using System;

    public class InvalidTableSchemeException : Exception
    {
        #region Constructors

        public InvalidTableSchemeException()
        {
        }

        public InvalidTableSchemeException(string message)
            : base(message)
        {
        }

        public InvalidTableSchemeException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}