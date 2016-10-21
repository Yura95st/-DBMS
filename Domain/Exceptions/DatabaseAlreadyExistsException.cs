namespace Domain.Exceptions
{
    using System;

    public class DatabaseAlreadyExistsException : Exception
    {
        #region Constructors

        public DatabaseAlreadyExistsException()
        {
        }

        public DatabaseAlreadyExistsException(string message)
            : base(message)
        {
        }

        public DatabaseAlreadyExistsException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}