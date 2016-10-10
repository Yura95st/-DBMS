namespace App.Validations.Abstract
{
    using App.Models;

    public interface IDatabaseValidation
    {
        void CheckTableScheme(TableScheme tableScheme);

        bool IsValidDatabaseName(string dbName);

        bool DoesRowFitTable(Table table, Row row);
    }
}