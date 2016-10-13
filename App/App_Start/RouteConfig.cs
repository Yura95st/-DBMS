namespace App
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(name: null, url: "databases/{dbName}/{tableName}/editRow/{rowId}",
                defaults: new { controller = "Home", action = "EditRow" });

            routes.MapRoute(name: null, url: "databases/{dbName}/{tableName}/addRow",
                defaults: new { controller = "Home", action = "AddRow" });

            routes.MapRoute(name: null, url: "databases/{dbName}/{tableName}/scheme",
                defaults: new { controller = "Home", action = "ShowTableScheme" });

            routes.MapRoute(name: null, url: "databases/{dbName}/{tableName}",
                defaults: new { controller = "Home", action = "ShowTable" });

            routes.MapRoute(name: null, url: "databases/{dbName}",
                defaults: new { controller = "Home", action = "ShowDatabase" });

            routes.MapRoute(name: null, url: "databases", defaults: new { controller = "Home", action = "Index" });

            routes.MapRoute(name: null, url: "{controller}/{action}", defaults: new { controller = "Home", action = "Index" });
        }
    }
}