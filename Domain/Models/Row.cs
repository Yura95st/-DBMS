namespace Domain.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public class Row
    {
        public Row()
        {
            this.Value = new List<string>();
        }

        [DataMember]
        public int Id
        {
            get;
            set;
        }

        [DataMember]
        public IList<string> Value
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
            if (this.Id != other.Id)
            {
                return false;
            }

            if (this.Value == null)
            {
                return other.Value == null;
            }

            return this.Value.SequenceEqual(other.Value);
        }
    }
}