namespace App.Exceptions
{
    using System;

    public class DbServiceException : Exception
    {
        #region Constructors

        public DbServiceException()
        {
        }

        public DbServiceException(string message)
            : base(message)
        {
        }

        public DbServiceException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}