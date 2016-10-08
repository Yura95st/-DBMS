namespace App.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class Table
    {
        public Table()
        {
            this.Attributes = new List<Attribute>();
            this.Rows = new Dictionary<int, Row>();
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

        public int NextRowId
        {
            get;
            set;
        }

        public IDictionary<int, Row> Rows
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj))
            {
                return false;
            }
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return this.Equals((Table)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (this.Attributes != null ? this.Attributes.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.Name != null ? this.Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.NextRowId;
                hashCode = (hashCode * 397) ^ (this.Rows != null ? this.Rows.GetHashCode() : 0);
                return hashCode;
            }
        }

        protected bool Equals(Table other)
        {
            return this.Attributes.SequenceEqual(other.Attributes) && string.Equals(this.Name, other.Name)
                && this.NextRowId == other.NextRowId && this.Rows.SequenceEqual(other.Rows);
        }
    }
}