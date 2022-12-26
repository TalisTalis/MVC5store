using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVC_magic_store
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

            routes.MapRoute("Pages", "{page}", new { controller = "Pages", action = "Index" }, new[] { "MVC_magic_store.Controllers" });
            
            routes.MapRoute("PagesMenuPartial", "Pages/PagesMenuPartial", new { controller = "Pages", action = "PagesMenuPartial" }, new[] { "MVC_magic_store.Controllers" });

            routes.MapRoute("Default", "", new { controller = "Pages", action = "Index" }, new[] { "MVC_magic_store.Controllers" });
        }
    }
}
