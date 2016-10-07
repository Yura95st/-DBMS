namespace App.Models
{
    using System.Collections.Generic;

    public class Database
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