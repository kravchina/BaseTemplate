using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using BaseTemplate.Web.App_Start;

namespace BaseTemplate.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            NinjectBindings.SetDependencyResolver(config);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
