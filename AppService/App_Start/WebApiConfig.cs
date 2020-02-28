using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AppService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            //config.Routes.MapHttpRoute(
            //    name: "chargeProduct1",
            //    routeTemplate: "api/{controller}/{productId}/{month}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            //config.Routes.MapHttpRoute(
            //    name: "chargeProduct2",
            //    routeTemplate: "api/{controller}/{productId}/{smsCode}/{inDate}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            //config.Routes.MapHttpRoute(
            //    name: "getSalesCenter",
            //    routeTemplate: "api/{controller}/{area}/{branchType}/{service}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            //config.Routes.MapHttpRoute(
            //    name: "getUserInfo",
            //    routeTemplate: "api/{controller}/{counter}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            //config.Routes.MapHttpRoute(
            //    name: "newOrder",
            //    routeTemplate: "api/{controller}/{orderType}/{userName}/{phoneNo}/{districtCode}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            //config.Routes.MapHttpRoute(
            //    name: "productList",
            //    routeTemplate: "api/{controller}/{productId}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            //config.Routes.MapHttpRoute(
            //    name: "vodList",
            //    routeTemplate: "api/{controller}/{productId}/{indate}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            //config.Routes.MapHttpRoute(
            //    name: "vodList1",
            //    routeTemplate: "api/{controller}/contentId={contentId}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
        }
    }
}
