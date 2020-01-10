using RestClient.Net.Abstractions;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestClient.Net
{
    /// <summary>
    /// Abstraction responsible for converting rest requests with data in to HttpRequestMessages
    /// </summary>
    public interface IRequestConverter
    {
        TimeSpan Timeout { get; set; }
        Uri BaseUri { get; set; }

        /// <summary>
        /// Convert rest request with data in to HttpRequestMessages
        /// </summary>
        HttpRequestMessage GetHttpRequestMessage<TRequestBody>(Request<TRequestBody> request, byte[] requestBodyData);

        Task<HttpResponseMessage> SendAsync<TRequestBody>(Request<TRequestBody> request, IRequestConverter requestConverter, byte[] requestBodyData);
    }
}
