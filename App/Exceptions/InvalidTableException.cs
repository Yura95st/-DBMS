namespace App.Exceptions
{
    using System;

    public class InvalidTableException : Exception
    {
        #region Constructors

        public InvalidTableException()
        {
        }

        public InvalidTableException(string message)
            : base(message)
        {
        }

        public InvalidTableException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}