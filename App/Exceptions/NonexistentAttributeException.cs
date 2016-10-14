namespace App.Exceptions
{
    using System;

    public class NonexistentAttributeException : Exception
    {
        #region Constructors

        public NonexistentAttributeException()
        {
        }

        public NonexistentAttributeException(string message)
            : base(message)
        {
        }

        public NonexistentAttributeException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}