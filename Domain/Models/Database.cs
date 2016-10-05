namespace Domain.Models
{
    using System.Collections.Generic;

    internal class Database
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