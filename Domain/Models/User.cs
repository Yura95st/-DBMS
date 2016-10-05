namespace Domain.Models
{
    using Domain.Enums;

    internal class User
    {
        public string Name
        {
            get;
            set;
        }

        public UserType Type
        {
            get;
            set;
        }
    }
}