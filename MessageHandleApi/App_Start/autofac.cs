﻿using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using LMS.Common.Service;
using LMS.Common.Service.Interface;

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
            builder.RegisterType<ElsService>().As<IElsService>().InstancePerLifetimeScope();
            builder.RegisterType<DbService>().As<IDbService>().InstancePerLifetimeScope();
            builder.RegisterType<AzureStorageService>().As<IAzureStorageService>().InstancePerLifetimeScope();
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}