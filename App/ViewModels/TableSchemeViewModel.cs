namespace App.ViewModels
{
    using System.Collections.Generic;

    using Domain.Models;

    public class TableSchemeViewModel
    {
        public IEnumerable<Attribute> Attributes
        {
            get;
            set;
        }

        public string DbName
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }
    }
}