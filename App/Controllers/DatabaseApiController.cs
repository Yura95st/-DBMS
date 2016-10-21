namespace App.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;

    using Domain.DTOs;
    using Domain.Exceptions;
    using Domain.Models;
    using Domain.Services.Abstract;
    using Domain.Utils;

    [RoutePrefix("api/databases")]
    public class DatabaseApiController : ApiController
    {
        private readonly IDatabaseService _databaseService;

        public DatabaseApiController(IDatabaseService databaseService)
        {
            Guard.NotNull(databaseService, "databaseService");

            this._databaseService = databaseService;
        }

        [Route("{dbName}/{tableName}")]
        [HttpPost]
        public IHttpActionResult AddRow(string dbName, string tableName, Row row)
        {
            try
            {
                this._databaseService.AddRow(dbName, tableName, row);

                return this.Ok();
            }
            catch (ArgumentException)
            {
                return this.BadRequest();
            }
            catch (DatabaseNotFoundException)
            {
                return this.BadRequest("Database with such name does not exist.");
            }
            catch (TableNotFoundException)
            {
                return this.BadRequest("Table with such name does not exist.");
            }
            catch (InvalidRowException)
            {
                return this.BadRequest("Row is invalid.");
            }
            catch (DbServiceException)
            {
                return this.InternalServerError();
            }
        }

        [Route("")]
        [HttpPost]
        public IHttpActionResult CreateDatabase(string dbName)
        {
            try
            {
                this._databaseService.CreateDatabase(dbName);

                return this.CreatedAtRoute("GetDatabase", new { dbName }, dbName);
            }
            catch (ArgumentException)
            {
                return this.BadRequest();
            }
            catch (InvalidDatabaseNameException)
            {
                return this.BadRequest("Invalid name format.");
            }
            catch (DatabaseAlreadyExistsException)
            {
                return this.Conflict();
            }
            catch (DbServiceException)
            {
                return this.InternalServerError();
            }
        }

        [Route("{dbName}")]
        [HttpPost]
        public IHttpActionResult CreateTable(string dbName, TableScheme tableScheme)
        {
            try
            {
                this._databaseService.CreateTable(dbName, tableScheme);

                return this.CreatedAtRoute("GetTable", new { dbName, tableName = tableScheme.Name }, tableScheme);
            }
            catch (ArgumentException)
            {
                return this.BadRequest();
            }
            catch (DatabaseNotFoundException)
            {
                return this.BadRequest("Database with such name does not exist.");
            }
            catch (InvalidTableSchemeException)
            {
                return this.BadRequest("Invalid table scheme.");
            }
            catch (TableAlreadyExistsException)
            {
                return this.Conflict();
            }
            catch (DbServiceException)
            {
                return this.InternalServerError();
            }
        }

        [Route("{dbName}/{tableName}")]
        [HttpDelete]
        public IHttpActionResult DeleteRow(string dbName, string tableName, int rowId)
        {
            try
            {
                this._databaseService.DeleteRow(dbName, tableName, rowId);

                return this.Ok();
            }
            catch (ArgumentException)
            {
                return this.BadRequest();
            }
            catch (DatabaseNotFoundException)
            {
                return this.NotFound();
            }
            catch (TableNotFoundException)
            {
                return this.NotFound();
            }
            catch (RowNotFoundException)
            {
                return this.NotFound();
            }
            catch (DbServiceException)
            {
                return this.InternalServerError();
            }
        }

        [Route("")]
        [HttpDelete]
        public IHttpActionResult DropDatabase(string dbName)
        {
            try
            {
                this._databaseService.DropDatabase(dbName);

                return this.Ok();
            }
            catch (ArgumentException)
            {
                return this.BadRequest();
            }
            catch (DatabaseNotFoundException)
            {
                return this.NotFound();
            }
            catch (DbServiceException)
            {
                return this.InternalServerError();
            }
        }

        [Route("{dbName}")]
        [HttpDelete]
        public IHttpActionResult DropTable(string dbName, string tableName)
        {
            try
            {
                this._databaseService.DropTable(dbName, tableName);

                return this.Ok();
            }
            catch (ArgumentException)
            {
                return this.BadRequest();
            }
            catch (DatabaseNotFoundException)
            {
                return this.NotFound();
            }
            catch (TableNotFoundException)
            {
                return this.NotFound();
            }
            catch (DbServiceException)
            {
                return this.InternalServerError();
            }
        }

        [Route("{dbName}", Name = "GetDatabase")]
        [HttpGet]
        public IHttpActionResult GetDatabase(string dbName)
        {
            try
            {
                Database database = this._databaseService.GetDatabase(dbName);
                if (database == null)
                {
                    return this.NotFound();
                }

                return this.Ok(DatabaseApiController.GetDatabaseDto(database));
            }
            catch (ArgumentException)
            {
                return this.BadRequest();
            }
            catch (DbServiceException)
            {
                return this.InternalServerError();
            }
        }

        [Route("")]
        [HttpGet]
        public IHttpActionResult GetDatabasesNames()
        {
            IEnumerable<string> dbNames = this._databaseService.GetDatabaseNames();

            return this.Ok(dbNames);
        }

        [Route("{dbName}/{tableName}", Name = "GetTable")]
        [HttpGet]
        public IHttpActionResult GetTable(string dbName, string tableName)
        {
            try
            {
                Table table = this._databaseService.GetTable(dbName, tableName);
                if (table == null)
                {
                    return this.NotFound();
                }

                return this.Ok(DatabaseApiController.GetTableDto(table));
            }
            catch (ArgumentException)
            {
                return this.BadRequest();
            }
            catch (DatabaseNotFoundException)
            {
                return this.NotFound();
            }
            catch (DbServiceException)
            {
                return this.InternalServerError();
            }
        }

        [Route("{dbName}/{tableName}/projection")]
        [HttpGet]
        public IHttpActionResult GetTableProjection(string dbName, string tableName,
                                                    [FromUri] IEnumerable<string> attributesNames)
        {
            try
            {
                Table table = this._databaseService.GetTableProjection(dbName, tableName, attributesNames);
                if (table == null)
                {
                    return this.NotFound();
                }

                return this.Ok(DatabaseApiController.GetTableDto(table));
            }
            catch (ArgumentException)
            {
                return this.BadRequest();
            }
            catch (DatabaseNotFoundException)
            {
                return this.NotFound();
            }
            catch (AttributeNotFoundException)
            {
                return this.BadRequest("Nonexistent attribute's name.");
            }
            catch (DbServiceException)
            {
                return this.InternalServerError();
            }
        }

        [Route("{dbName}/{tableName}")]
        [HttpPut]
        public IHttpActionResult UpdateRow(string dbName, string tableName, Row row)
        {
            try
            {
                this._databaseService.UpdateRow(dbName, tableName, row);

                return this.Ok();
            }
            catch (ArgumentException)
            {
                return this.BadRequest();
            }
            catch (DatabaseNotFoundException)
            {
                return this.NotFound();
            }
            catch (TableNotFoundException)
            {
                return this.NotFound();
            }
            catch (RowNotFoundException)
            {
                return this.NotFound();
            }
            catch (InvalidRowException)
            {
                return this.BadRequest("Row is invalid.");
            }
            catch (DbServiceException)
            {
                return this.InternalServerError();
            }
        }

        private static DatabaseDto GetDatabaseDto(Database database)
        {
            DatabaseDto dbDto = new DatabaseDto { Name = database.Name, TableNames = database.Tables.Keys };

            return dbDto;
        }

        private static TableDto GetTableDto(Table table)
        {
            TableDto tableDto = new TableDto { Name = table.Name, Attributes = table.Attributes, Rows = table.Rows.Values };

            return tableDto;
        }
    }
}