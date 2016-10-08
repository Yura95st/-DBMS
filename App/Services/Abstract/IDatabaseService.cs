namespace App.Services.Abstract
{
    using App.Models;

    public interface IDatabaseService
    {
        void AddRow(string dbName, string tableName, Row row);

        void CreateDatabase(string dbName);

        void CreateTable(string dbName, Table table);

        void DeleteRow(string dbName, string tableName, int rowId);

        void DropDatabase(string dbName);

        void DropTable(string dbName, string tableName);

        Database GetDatabase(string dbName);

        Table GetTable(string dbName, string tableName);

        void UpdateRow(string dbName, string tableName, Row row);
    }
}