namespace Domain.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class Table
    {
        public Table()
        {
            this.NextRowId = 1;
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
            if ((this.Attributes == null && other.Attributes != null) || (this.Attributes != null && other.Attributes == null))
            {
                return false;
            }

            if (this.Attributes != null && other.Attributes != null && !this.Attributes.SequenceEqual(other.Attributes))
            {
                return false;
            }

            if ((this.Rows == null && other.Rows != null) || (this.Rows != null && other.Rows == null))
            {
                return false;
            }

            if (this.Rows != null && other.Rows != null && !this.Rows.SequenceEqual(other.Rows))
            {
                return false;
            }

            return this.NextRowId == other.NextRowId && string.Equals(this.Name, other.Name);
        }
    }
}