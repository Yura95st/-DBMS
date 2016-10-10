namespace App.Validations.Abstract
{
    using App.Models;

    public interface IDatabaseValidation
    {
        void CheckAttribute(Attribute attribute);

        void CheckRow(Table table, Row row);

        void CheckTableScheme(TableScheme tableScheme);

        bool IsValidDatabaseName(string dbName);
    }
}