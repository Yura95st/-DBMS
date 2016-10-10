namespace App.Exceptions
{
    using System;

    public class InvalidAttributeException : Exception
    {
        #region Constructors

        public InvalidAttributeException()
        {
        }

        public InvalidAttributeException(string message)
            : base(message)
        {
        }

        public InvalidAttributeException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}