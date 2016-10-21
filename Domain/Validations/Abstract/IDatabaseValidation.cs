namespace Domain.Validations.Abstract
{
    using Domain.Models;

    public interface IDatabaseValidation
    {
        void CheckTableScheme(TableScheme tableScheme);

        bool IsValidDatabaseName(string dbName);

        bool DoesRowFitTable(Table table, Row row);
    }
}