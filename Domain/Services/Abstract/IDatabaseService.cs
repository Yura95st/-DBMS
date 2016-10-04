namespace Domain.Services.Abstract
{
    using Domain.Models;

    internal interface IDatabaseService
    {
        void CreateTable(Database db, Table table, User user);

        Database CreateDatabase(string dbName, User user);

        void DropDatabase(Database db, User user);

        void DropTable(Database db, string tableName, User user);

        Database GetDatabase(string dbName);

        Table GetTable(Database db, string tableName);

        void UpdateTable(Database db, Table table, User user);
    }
}