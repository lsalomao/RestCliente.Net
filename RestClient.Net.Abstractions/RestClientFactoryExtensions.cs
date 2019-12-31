﻿using System;

namespace RestClientDotNet.Abstractions
{
    public static class RestClientFactoryExtensions
    {
        public static IClient CreateRestClient(this IRestClientFactory restClientFactory)
        {
            if (restClientFactory == null) throw new ArgumentNullException(nameof(restClientFactory));
            return restClientFactory.CreateRestClient("RestClient");
        }
    }
}
