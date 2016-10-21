namespace Domain.Exceptions
{
    using System;

    public class DbRepositoryException : Exception
    {
        #region Constructors

        public DbRepositoryException()
        {
        }

        public DbRepositoryException(string message)
            : base(message)
        {
        }

        public DbRepositoryException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}