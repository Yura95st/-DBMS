namespace App.Exceptions
{
    using System;

    public class InvalidDatabaseNameException : Exception
    {
        #region Constructors

        public InvalidDatabaseNameException()
        {
        }

        public InvalidDatabaseNameException(string message)
            : base(message)
        {
        }

        public InvalidDatabaseNameException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}