namespace WcfService
{
    using System;
    using System.Collections.Generic;

    using Domain.DTOs;
    using Domain.Models;
    using Domain.Services.Abstract;
    using Domain.Utils;

    using WcfService.Contracts;

    public class DbWcfService : IDbWcfService
    {
        private readonly IDatabaseService _databaseService;

        public DbWcfService(IDatabaseService databaseService)
        {
            Guard.NotNull(databaseService, "databaseService");

            this._databaseService = databaseService;
        }

        #region IDbWcfService Members

        public DatabaseDto GetDatabase(string dbName)
        {
            Database database = this._databaseService.GetDatabase(dbName);

            return database == null ? null : DatabaseDto.CreateFromDatabase(database);
        }

        public IEnumerable<string> GetDatabasesNames()
        {
            IEnumerable<string> dbNames = this._databaseService.GetDatabaseNames();

            return dbNames;
        }

        public TableDto GetTable(string dbName, string tableName)
        {
            Table table = this._databaseService.GetTable(dbName, tableName);

            return table == null ? null : TableDto.CreateFromTable(table);
        }

        public TableDto GetTableProjection(string dbName, string tableName, IEnumerable<string> attributesNames)
        {
            Table table = this._databaseService.GetTableProjection(dbName, tableName, attributesNames);

            return table == null ? null : TableDto.CreateFromTable(table);
        }

        #endregion
    }
}