namespace App.Exceptions
{
    using System;

    public class InvalidRowException : Exception
    {
        #region Constructors

        public InvalidRowException()
        {
        }

        public InvalidRowException(string message)
            : base(message)
        {
        }

        public InvalidRowException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}