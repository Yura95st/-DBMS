namespace Domain.Models
{
    using System.Collections.Generic;

    internal class Database
    {
        private User _creator;

        private string _name;

        private Dictionary<string, Table> _tables;
    }
}