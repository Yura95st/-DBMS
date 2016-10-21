namespace Domain.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Utils;

    public class TableScheme
    {
        public TableScheme(string name, IList<Attribute> attributes)
        {
            Guard.NotNull(name, "name");
            Guard.NotNull(attributes, "attributes");

            this.Name = name;
            this.Attributes = attributes.ToList();
        }

        public IList<Attribute> Attributes
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }
    }
}