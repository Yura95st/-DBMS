namespace Domain.Exceptions
{
    using System;

    public class DatabaseNotFoundException : Exception
    {
        #region Constructors

        public DatabaseNotFoundException()
        {
        }

        public DatabaseNotFoundException(string message)
            : base(message)
        {
        }

        public DatabaseNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}