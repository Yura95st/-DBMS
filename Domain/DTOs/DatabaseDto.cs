namespace Domain.DTOs
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    using Domain.Models;
    using Domain.Utils;

    [DataContract]
    public class DatabaseDto
    {
        public DatabaseDto()
        {
            this.TableNames = Enumerable.Empty<string>();
        }

        [DataMember]
        public string Name
        {
            get;
            set;
        }

        [DataMember]
        public IEnumerable<string> TableNames
        {
            get;
            set;
        }

        public static DatabaseDto CreateFromDatabase(Database database)
        {
            Guard.NotNull(database, "database");

            DatabaseDto dbDto = new DatabaseDto { Name = database.Name, TableNames = database.Tables.Keys };

            return dbDto;
        }
    }
}