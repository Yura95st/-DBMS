namespace App.Models
{
    using System.Collections.Generic;

    public class Row
    {
        public Row()
        {
            this.Value = new Dictionary<Attribute, string>();
        }

        public int Id
        {
            get;
            set;
        }

        public IDictionary<Attribute, string> Value
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
            return this.Equals((Row)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Id * 397) ^ (this.Value != null ? this.Value.GetHashCode() : 0);
            }
        }

        protected bool Equals(Row other)
        {
            return this.Id == other.Id && object.Equals(this.Value, other.Value);
        }
    }
}