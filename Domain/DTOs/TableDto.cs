namespace Domain.DTOs
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    using Domain.Models;
    using Domain.Utils;

    [DataContract]
    public class TableDto
    {
        public TableDto()
        {
            this.Attributes = new List<Attribute>();
            this.Rows = Enumerable.Empty<Row>();
        }

        [DataMember]
        public IList<Attribute> Attributes
        {
            get;
            set;
        }

        [DataMember]
        public string Name
        {
            get;
            set;
        }

        [DataMember]
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