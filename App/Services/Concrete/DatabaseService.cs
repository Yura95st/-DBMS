namespace App.Services.Concrete
{
    using App.Models;
    using App.Services.Abstract;

    public class DatabaseService : IDatabaseService
    {
        #region IDatabaseService Members

        public Database CreateDatabase(string dbName)
        {
            throw new System.NotImplementedException();
        }

        public void CreateTable(Database db, Table table)
        {
            throw new System.NotImplementedException();
        }

        public void DropDatabase(Database db)
        {
            throw new System.NotImplementedException();
        }

        public void DropTable(Database db, string tableName)
        {
            throw new System.NotImplementedException();
        }

        public Database GetDatabase(string dbName)
        {
            throw new System.NotImplementedException();
        }

        public Table GetTable(Database db, string tableName)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateTable(Database db, Table table)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}