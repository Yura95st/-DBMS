namespace App.DTOs
{
    using System.Collections.Generic;
    using System.Linq;

    public class DatabaseDto
    {
        public DatabaseDto()
        {
            this.TableNames = Enumerable.Empty<string>();
        }

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