namespace Domain.DTOs
{
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Models;
    using Domain.Utils;

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

        public static TableDto CreateFromTable(Table table)
        {
            Guard.NotNull(table, "table");

            TableDto tableDto = new TableDto { Name = table.Name, Attributes = table.Attributes, Rows = table.Rows.Values };

            return tableDto;
        }
    }
}