#if !NET45

using Microsoft.Extensions.Logging;
using RestClient.Net.Abstractions;
using RestClient.Net.Abstractions.Extensions;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RestClient.Net
{
    public sealed partial class Client
    {

        /// <summary>
        /// Construct a client.
        /// </summary>
        /// <param name="serializationAdapter">The serialization adapter for serializing/deserializing http content bodies. Defaults to JSON and adds the default Content-Type header for JSON</param>
        /// <param name="name">The of the client instance. This is also passed to the HttpClient factory to get or create HttpClient instances</param>
        /// <param name="baseUri">The base Url for the client. Specify this if the client will be used for one Url only</param>
        /// <param name="defaultRequestHeaders">Default headers to be sent with http requests</param>
        /// <param name="logger">Logging abstraction that will trace request/response data and log events</param>
        /// <param name="httpClientFactory">The IHttpClientFactory instance that is used for getting or creating HttpClient instances when the SendAsync call is made</param>
        /// <param name="sendHttpRequestFunc">The Func responsible for performing the SendAsync method on HttpClient. This can replaced in the constructor in order to implement retries and so on.</param>
        /// <param name="requestConverter">IRequestConverter instance responsible for converting rest requests to http requests</param>
        public Client(

           ISerializationAdapter serializationAdapter = null,

            string name = null,
            Uri baseUri = null,
            IHeadersCollection defaultRequestHeaders = null,
            ILogger logger = null,
            IHttpClientFactory httpClientFactory = null,
            Func<HttpClient, Func<HttpRequestMessage>, ILogger, CancellationToken, Task<HttpResponseMessage>> sendHttpRequestFunc = null,
            IRequestConverter requestConverter = null)
        {
            DefaultRequestHeaders = defaultRequestHeaders ?? new RequestHeadersCollection();

            if (serializationAdapter == null)
            {
                SerializationAdapter = new JsonSerializationAdapter();
                this.SetJsonContentTypeHeader();
            }
            else
            {
                SerializationAdapter = serializationAdapter;
            }

            Logger = logger;
            BaseUri = baseUri;
            Name = name ?? Guid.NewGuid().ToString();
            RequestConverter = requestConverter ?? new DefaultRequestConverter(Logger);
            HttpClientFactory = httpClientFactory ?? new DefaultHttpClientFactory();
            _sendHttpRequestFunc = sendHttpRequestFunc ?? DefaultSendHttpRequestMessageFunc;
        }

        /// <summary>
        /// Construct a client
        /// </summary>
        /// <param name="baseUri">The base Url for the client. Specify this if the client will be used for one Url only</param>
        public Client(
            Uri baseUri)
        : this(
            null,
            null,
            baseUri)
        {
        }
    }
}

#endif