namespace App.Services.Abstract
{
    using App.Models;

    public interface IDatabaseService
    {
        void CreateTable(string dbName, Table table);

        void CreateDatabase(string dbName);

        void DropDatabase(string dbName);

        void DropTable(string dbName, string tableName);

        Database GetDatabase(string dbName);

        Table GetTable(string dbName, string tableName);

        void UpdateTable(string dbName, Table table);
    }
}