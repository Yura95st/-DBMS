namespace Domain.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class Database
    {
        public Database()
        {
            this.Tables = new Dictionary<string, Table>();
        }

        public string Name
        {
            get;
            set;
        }

        public IDictionary<string, Table> Tables
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
            return this.Equals((Database)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (this.Name != null ? this.Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.Tables != null ? this.Tables.GetHashCode() : 0);
                return hashCode;
            }
        }

        protected bool Equals(Database other)
        {
            if ((this.Tables == null && other.Tables != null) || (this.Tables != null && other.Tables == null))
            {
                return false;
            }

            if (this.Tables != null && other.Tables != null && !this.Tables.SequenceEqual(other.Tables))
            {
                return false;
            }

            return string.Equals(this.Name, other.Name);
        }
    }
}