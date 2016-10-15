namespace App.Exceptions
{
    using System;

    public class AttributeNotFoundException : Exception
    {
        #region Constructors

        public AttributeNotFoundException()
        {
        }

        public AttributeNotFoundException(string message)
            : base(message)
        {
        }

        public AttributeNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}