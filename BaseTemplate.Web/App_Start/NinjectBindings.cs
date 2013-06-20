using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using BaseTemplate.Infrastructure;
using BaseTemplate.Infrastructure.DataBaseContext;
using Ninject;
using Ninject.Web.Common;

namespace BaseTemplate.Web.App_Start
{
    public static class NinjectBindings
    {
        public static void SetDependencyResolver(HttpConfiguration config)
        {
            var kernel = new StandardKernel();

            Bind(kernel);

            config.DependencyResolver = new NinjectDependencyResolver(kernel);
        }

        public static void Bind(IKernel kernel)
        {
            // Товарищ! Bind'и здесь!

            kernel.Bind<EfDbContext>().ToSelf().InRequestScope();
        }
    } 
}