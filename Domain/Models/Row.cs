namespace Domain.Models
{
    using System.Collections.Generic;

    internal class Row
    {
        public IDictionary<Column, string> Value
        {
            get;
            set;
        }
    }
}