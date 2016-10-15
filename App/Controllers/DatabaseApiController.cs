namespace App.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;

    using App.DTOs;
    using App.Exceptions;
    using App.Models;
    using App.Services.Abstract;
    using App.Utils;

    [RoutePrefix("api/databases")]
    public class DatabaseApiController : ApiController
    {
        private readonly IDatabaseService _databaseService;

        public DatabaseApiController(IDatabaseService databaseService)
        {
            Guard.NotNull(databaseService, "databaseService");

            this._databaseService = databaseService;
        }

        [Route("{dbName}")]
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

        private static DatabaseDto GetDatabaseDto(Database database)
        {
            DatabaseDto dbDto = new DatabaseDto { Name = database.Name, TableNames = database.Tables.Keys };

            return dbDto;
        }
    }
}