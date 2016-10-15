namespace App.Exceptions
{
    using System;

    public class InvalidTableNameException : Exception
    {
        #region Constructors

        public InvalidTableNameException()
        {
        }

        public InvalidTableNameException(string message)
            : base(message)
        {
        }

        public InvalidTableNameException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}