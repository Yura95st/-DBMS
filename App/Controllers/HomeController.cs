namespace App.Controllers
{
    using System.Web.Mvc;

    using Domain.DTOs;
    using Domain.Models;
    using Domain.Services.Abstract;
    using Domain.Utils;

    public class HomeController : Controller
    {
        private readonly IDatabaseService _databaseService;

        public HomeController(IDatabaseService databaseService)
        {
            Guard.NotNull(databaseService, "databaseService");

            this._databaseService = databaseService;
        }

        public ActionResult Index()
        {
            this.ViewBag.dbNames = this._databaseService.GetDatabaseNames();

            return this.View();
        }

        public ActionResult ShowDatabase(string dbName)
        {
            Database db = this._databaseService.GetDatabase(dbName);

            if (db == null)
            {
                return new HttpNotFoundResult("Database not found.");
            }

            return this.View(new DatabaseDto { Name = db.Name, TableNames = db.Tables.Keys });
        }

        public ActionResult ShowTable(string dbName, string tableName)
        {
            Table table = this._databaseService.GetTable(dbName, tableName);

            if (table == null)
            {
                return new HttpNotFoundResult("Table not found.");
            }

            return this.View(table);
        }

        public ActionResult ShowTableScheme(string dbName, string tableName)
        {
            Table table = this._databaseService.GetTable(dbName, tableName);

            if (table == null)
            {
                return new HttpNotFoundResult("Table not found.");
            }

            TableScheme tableScheme = new TableScheme(table.Name, table.Attributes);

            return this.View(tableScheme);
        }
    }
}