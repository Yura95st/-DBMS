namespace App.Models
{
    public class Attribute
    {
        public string Name
        {
            get;
            set;
        }

        public string Type
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
            return this.Equals((Attribute)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.Name != null ? this.Name.GetHashCode() : 0) * 397)
                    ^ (this.Type != null ? this.Type.GetHashCode() : 0);
            }
        }

        protected bool Equals(Attribute other)
        {
            return string.Equals(this.Name, other.Name) && string.Equals(this.Type, other.Type);
        }
    }
}