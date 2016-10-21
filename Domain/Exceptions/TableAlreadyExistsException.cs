namespace Domain.Exceptions
{
    using System;

    public class TableAlreadyExistsException : Exception
    {
        #region Constructors

        public TableAlreadyExistsException()
        {
        }

        public TableAlreadyExistsException(string message)
            : base(message)
        {
        }

        public TableAlreadyExistsException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}