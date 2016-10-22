namespace App.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Mvc;

    using App.ViewModels;

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

            DatabaseViewModel dbViewModel = new DatabaseViewModel { Name = db.Name, TableNames = db.Tables.Keys.ToList() };

            return this.View(dbViewModel);
        }

        public ActionResult ShowTable(string dbName, string tableName)
        {
            Table table = this._databaseService.GetTable(dbName, tableName);

            if (table == null)
            {
                return new HttpNotFoundResult("Table not found.");
            }

            TableViewModel tableViewModel = HomeController.GetTableViewModel(dbName, table);

            return this.View(tableViewModel);
        }

        public ActionResult ShowTableProjection(string dbName, string tableName,
                                                [FromUri] IEnumerable<string> attributesNames)
        {
            Table table = this._databaseService.GetTableProjection(dbName, tableName, attributesNames);
            if (table == null)
            {
                return new HttpNotFoundResult("Table not found.");
            }

            TableViewModel tableViewModel = HomeController.GetTableViewModel(dbName, table);

            return this.View("ShowTable", tableViewModel);
        }

        public ActionResult ShowTableScheme(string dbName, string tableName)
        {
            Table table = this._databaseService.GetTable(dbName, tableName);

            if (table == null)
            {
                return new HttpNotFoundResult("Table not found.");
            }

            TableSchemeViewModel tableSchemeViewModel = new TableSchemeViewModel
                { Name = table.Name, DbName = dbName, Attributes = table.Attributes.ToList() };

            return this.View(tableSchemeViewModel);
        }

        private static TableViewModel GetTableViewModel(string dbName, Table table)
        {
            TableViewModel tableViewModel = new TableViewModel
            {
                Scheme =
                    new TableSchemeViewModel { Name = table.Name, DbName = dbName, Attributes = table.Attributes.ToList() },
                Rows = table.Rows.Values.ToList()
            };
            return tableViewModel;
        }
    }
}