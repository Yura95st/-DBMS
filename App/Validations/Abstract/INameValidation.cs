namespace App.Validations.Abstract
{
    public interface INameValidation
    {
        bool IsValidAttributeName(string attributeName);

        bool IsValidDatabaseName(string dbName);

        bool IsValidTableName(string tableName);
    }
}