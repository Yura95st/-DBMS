namespace App.DTOs
{
    using System.Collections.Generic;
    using System.Linq;

    using App.Models;

    public class TableDto
    {
        public TableDto()
        {
            this.Attributes = new List<Attribute>();
            this.Rows = Enumerable.Empty<Row>();
        }

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