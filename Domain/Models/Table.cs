namespace Domain.Models
{
    using System.Collections.Generic;

    internal class Table
    {
        private List<Column> _columns;

        private string _name;

        private List<Row> _rows;
    }
}