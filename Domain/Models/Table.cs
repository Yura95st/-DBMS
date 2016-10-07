namespace Domain.Models
{
    using System.Collections.Generic;

    public class Table
    {
        public IList<Attribute> Attributes
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public IEnumerable<Row> Rows
        {
            get;
            set;
        }
    }
}