namespace WcfService.Contracts
{
    using System.Collections.Generic;
    using System.ServiceModel;

    using Domain.DTOs;

    [ServiceContract]
    public interface IDbWcfService
    {
        [OperationContract]
        DatabaseDto GetDatabase(string dbName);

        [OperationContract]
        IEnumerable<string> GetDatabasesNames();

        [OperationContract]
        TableDto GetTable(string dbName, string tableName);

        [OperationContract]
        TableDto GetTableProjection(string dbName, string tableName, IEnumerable<string> attributesNames);
    }
}