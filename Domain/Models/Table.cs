namespace Domain.Models
{
    using System.Collections.Generic;

    internal class Table
    {
        public IList<Column> Columns
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