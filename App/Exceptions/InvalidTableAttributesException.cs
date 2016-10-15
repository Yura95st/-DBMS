namespace App.Exceptions
{
    using System;

    public class InvalidTableAttributesException : Exception
    {
        #region Constructors

        public InvalidTableAttributesException()
        {
        }

        public InvalidTableAttributesException(string message)
            : base(message)
        {
        }

        public InvalidTableAttributesException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}