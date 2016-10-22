namespace App.ViewModels
{
    using System.Collections.Generic;

    public class DatabaseViewModel
    {
        public string Name
        {
            get;
            set;
        }

        public IEnumerable<string> TableNames
        {
            get;
            set;
        }
    }
}