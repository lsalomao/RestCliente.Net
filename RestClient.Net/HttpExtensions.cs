using RestClientDotNet.Abstractions;
using System;
using System.Net.Http;

namespace RestClientDotNet
{
    public static class HttpExtensions
    {
        public static HttpVerb ToHttpVerb(this HttpMethod httpMethod)
        {
            if (httpMethod == null) throw new ArgumentNullException(nameof(httpMethod));

            if (httpMethod == HttpMethod.Get) return HttpVerb.Get;
            if (httpMethod == HttpMethod.Put) return HttpVerb.Put;
            if (httpMethod == HttpMethod.Post) return HttpVerb.Post;
            if (httpMethod == HttpMethod.Delete) return HttpVerb.Delete;
            if (string.Compare(httpMethod.Method, "PATCH", StringComparison.OrdinalIgnoreCase) == 0) return HttpVerb.Patch;

            throw new NotImplementedException();
        }
    }
}
