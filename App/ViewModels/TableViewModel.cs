namespace App.ViewModels
{
    using System.Collections.Generic;

    using Domain.Models;

    public class TableViewModel
    {
        public TableSchemeViewModel Scheme
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