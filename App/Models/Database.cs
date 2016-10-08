namespace App.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class Database
    {
        public Database()
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
                return ((this.Name != null ? this.Name.GetHashCode() : 0) * 397)
                    ^ (this.TableNames != null ? this.TableNames.GetHashCode() : 0);
            }
        }

        protected bool Equals(Database other)
        {
            return string.Equals(this.Name, other.Name) && this.TableNames.SequenceEqual(other.TableNames);
        }
    }
}