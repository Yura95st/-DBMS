namespace App.Validations.Abstract
{
    using App.Models;

    public interface IDatabaseValidation
    {
        void CheckAttribute(Attribute attribute);

        void CheckTable(Table table);

        bool IsValidDatabaseName(string dbName);
    }
}