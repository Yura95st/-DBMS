namespace Domain.Exceptions
{
    using System;

    public class TableNotFoundException : Exception
    {
        #region Constructors

        public TableNotFoundException()
        {
        }

        public TableNotFoundException(string message)
            : base(message)
        {
        }

        public TableNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}