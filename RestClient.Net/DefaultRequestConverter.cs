using RestClient.Net.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

#pragma warning disable CA2000

namespace RestClient.Net
{
    public class DefaultRequestConverter : IRequestConverter
    {
        public TimeSpan Timeout { get; set; }
        public Uri BaseUri { get; set; }
        public string Name { get; set; }

        public DefaultRequestConverter(IHttpClientFactory httpClientFactory) : this(httpClientFactory, null)
        {
        }

        public DefaultRequestConverter(IHttpClientFactory httpClientFactory, string name)
        {
            Name = name ?? "RestClient";
            HttpClientFactory = httpClientFactory;
        }

        #region Public Methods
        public static readonly List<HttpRequestMethod> UpdateHttpRequestMethods = new List<HttpRequestMethod> { HttpRequestMethod.Put, HttpRequestMethod.Post, HttpRequestMethod.Patch };
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the current IHttpClientFactory instance that is used for getting or creating HttpClient instances when the SendAsync call is made
        /// </summary>
        public IHttpClientFactory HttpClientFactory { get; }

        public Task<HttpResponseMessage> SendAsync<T>(Request<T> request, IRequestConverter requestConverter, byte[] requestBodyData)
        {
            var httpClient = HttpClientFactory.CreateClient(Name);

            //Note: if HttpClient naming is not handled properly, this may alter the HttpClient of another RestClient
            if (httpClient.Timeout != Timeout && Timeout != default) httpClient.Timeout = Timeout;
            if (httpClient.BaseAddress != BaseUri && BaseUri != null) httpClient.BaseAddress = BaseUri;

            if (request == null) throw new ArgumentNullException(nameof(request));
            if (requestConverter == null) throw new ArgumentNullException(nameof(requestConverter));

            var httpRequestMessage = requestConverter.GetHttpRequestMessage(request, requestBodyData);
            return httpClient.SendAsync(httpRequestMessage, request.CancellationToken);
        }

        public virtual HttpRequestMessage GetHttpRequestMessage<TRequestBody>(Request<TRequestBody> request, byte[] requestBodyData)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            HttpMethod httpMethod;
            if (string.IsNullOrEmpty(request.CustomHttpRequestMethod))
            {
                switch (request.HttpRequestMethod)
                {
                    case HttpRequestMethod.Get:
                        httpMethod = HttpMethod.Get;
                        break;
                    case HttpRequestMethod.Post:
                        httpMethod = HttpMethod.Post;
                        break;
                    case HttpRequestMethod.Put:
                        httpMethod = HttpMethod.Put;
                        break;
                    case HttpRequestMethod.Delete:
                        httpMethod = HttpMethod.Delete;
                        break;
                    case HttpRequestMethod.Patch:
                        httpMethod = new HttpMethod("PATCH");
                        break;
                    case HttpRequestMethod.Custom:
                        throw new NotImplementedException("CustomHttpRequestMethod must be specified for Custom Http Requests");
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                httpMethod = new HttpMethod(request.CustomHttpRequestMethod);
            }

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = httpMethod,
                RequestUri = request.Resource
            };

            ByteArrayContent httpContent = null;
            if (UpdateHttpRequestMethods.Contains(request.HttpRequestMethod))
            {
                httpContent = new ByteArrayContent(requestBodyData);
                httpRequestMessage.Content = httpContent;
            }

            foreach (var headerName in request.Headers?.Names)
            {
                if (string.Compare(headerName, "Content-Type", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    //Note: not sure why this is necessary...
                    //The HttpClient class seems to differentiate between content headers and request message headers, but this distinction doesn't exist in the real world...
                    //TODO: Other Content headers
                    httpContent?.Headers.Add("Content-Type", request.Headers[headerName]);
                }
                else
                {
                    httpRequestMessage.Headers.Add(headerName, request.Headers[headerName]);
                }
            }

            return httpRequestMessage;
        }
        #endregion
    }
}

#pragma warning restore CA2000