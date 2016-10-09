namespace App.Exceptions
{
    using System;

    public class RowNotFoundException : Exception
    {
        #region Constructors

        public RowNotFoundException()
        {
        }

        public RowNotFoundException(string message)
            : base(message)
        {
        }

        public RowNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}