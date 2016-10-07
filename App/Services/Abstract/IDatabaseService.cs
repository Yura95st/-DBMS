namespace App.Services.Abstract
{
    using App.Models;

    public interface IDatabaseService
    {
        void CreateTable(Database db, Table table);

        Database CreateDatabase(string dbName);

        void DropDatabase(Database db);

        void DropTable(Database db, string tableName);

        Database GetDatabase(string dbName);

        Table GetTable(Database db, string tableName);

        void UpdateTable(Database db, Table table);
    }
}