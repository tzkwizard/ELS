using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using LMS.service.Service;
//using MessageHandleApi.Service;

namespace MessageHandleApi
{
    public class Autofac
    {
        public static void Configure(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(typeof(Autofac).Assembly);
            builder.RegisterType<QueueService>().As<IQueueService>().InstancePerLifetimeScope();
            builder.RegisterType<ELSService>().As<IELSService>().InstancePerLifetimeScope();
            builder.RegisterType<DBService>().As<IDBService>().InstancePerLifetimeScope();
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}