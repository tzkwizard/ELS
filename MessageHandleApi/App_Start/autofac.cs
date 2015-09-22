using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using MessageHandleApi.Service;

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
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}