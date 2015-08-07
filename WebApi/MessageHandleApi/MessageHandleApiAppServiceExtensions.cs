using System;
using System.Net.Http;
using Microsoft.Azure.AppService;

namespace WebApi
{
    public static class MessageHandleApiAppServiceExtensions
    {
        public static MessageHandleApi CreateMessageHandleApi(this IAppServiceClient client)
        {
            return new MessageHandleApi(client.CreateHandler());
        }

        public static MessageHandleApi CreateMessageHandleApi(this IAppServiceClient client, params DelegatingHandler[] handlers)
        {
            return new MessageHandleApi(client.CreateHandler(handlers));
        }

        public static MessageHandleApi CreateMessageHandleApi(this IAppServiceClient client, Uri uri, params DelegatingHandler[] handlers)
        {
            return new MessageHandleApi(uri, client.CreateHandler(handlers));
        }

        public static MessageHandleApi CreateMessageHandleApi(this IAppServiceClient client, HttpClientHandler rootHandler, params DelegatingHandler[] handlers)
        {
            return new MessageHandleApi(rootHandler, client.CreateHandler(handlers));
        }
    }
}
