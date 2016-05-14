using ProStoneIMS2015.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ProStoneIMS2015
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Quote",
                url: "quote/{id}",
                defaults: new { controller = "Quote", action = "Index" },
                constraints: new { Id = @"\d+" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            //routes.MapRoute(
            //            name: "Default", url: "{controller}/{action}/{id}",
            //            defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
            //            constraints: new { TenantAccess = new TenantRouteConstraint() }
            //);
        }
    }
}
