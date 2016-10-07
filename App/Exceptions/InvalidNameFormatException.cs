namespace App.Exceptions
{
    using System;

    public class InvalidNameFormatException : Exception
    {
        #region Constructors

        public InvalidNameFormatException()
        {
        }

        public InvalidNameFormatException(string message)
            : base(message)
        {
        }

        public InvalidNameFormatException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}