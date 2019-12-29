using RestClientDotNet.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

#pragma warning disable CA2000

namespace RestClientDotNet
{
    public class DefaultHttpRequestProcessor : IHttpRequestProcessor
    {
        public static readonly List<HttpVerb> UpdateVerbs = new List<HttpVerb> { HttpVerb.Put, HttpVerb.Post, HttpVerb.Patch };

        public virtual HttpRequestMessage GetHttpRequestMessage<TRequestBody>(RestRequest<TRequestBody> restRequest, byte[] requestBodyData)
        {
            if (restRequest == null) throw new ArgumentNullException(nameof(restRequest));

            HttpMethod httpMethod;
            switch (restRequest.HttpVerb)
            {
                case HttpVerb.Get:
                    httpMethod = HttpMethod.Get;
                    break;
                case HttpVerb.Post:
                    httpMethod = HttpMethod.Post;
                    break;
                case HttpVerb.Put:
                    httpMethod = HttpMethod.Put;
                    break;
                case HttpVerb.Delete:
                    httpMethod = HttpMethod.Delete;
                    break;
                case HttpVerb.Patch:
                    httpMethod = new HttpMethod("PATCH");
                    break;
                default:
                    throw new NotImplementedException();
            }

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = httpMethod,
                RequestUri = restRequest.Resource,
            };

            ByteArrayContent httpContent = null;
            if (UpdateVerbs.Contains(restRequest.HttpVerb))
            {
                httpContent = new ByteArrayContent(requestBodyData);
                httpRequestMessage.Content = httpContent;
            }

            foreach (var headerName in restRequest.Headers.Names)
            {
                if (string.Compare(headerName, "Content-Type", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    //Note: not sure why this is necessary...
                    //The HttpClient class seems to differentiate between content headers and request message headers, but this distinction doesn't exist in the real world...
                    //TODO: Other Content headers
                    httpContent?.Headers.Add("Content-Type", restRequest.Headers[headerName]);
                }
                else
                {
                    httpRequestMessage.Headers.Add(headerName, restRequest.Headers[headerName]);
                }
            }

            return httpRequestMessage;
        }

        public async Task<RestResponseBase<TResponseBody>> GetRestResponse<TResponseBody>(HttpResponseMessage httpResponseMessage, byte[] responseBodyData, ISerializationAdapter serializationAdapter)
        {
            if (httpResponseMessage == null) throw new ArgumentNullException(nameof(httpResponseMessage));

            var restHeadersCollection = new RestResponseHeaders(httpResponseMessage.Headers);

            TResponseBody responseBody;
            try
            {
                responseBody = serializationAdapter.Deserialize<TResponseBody>(responseBodyData, restHeadersCollection);
            }
            catch (Exception ex)
            {
                throw new DeserializationException(Messages.ErrorMessageDeserialization, responseBodyData, ex);
            }

            var restResponse = new RestResponse<TResponseBody>
            (
                restHeadersCollection,
                (int)httpResponseMessage.StatusCode,
                httpResponseMessage.RequestMessage.RequestUri,
                httpResponseMessage.RequestMessage.Method.ToHttpVerb(),
                responseBodyData,
                responseBody,
                httpResponseMessage
            );

            return restResponse;
        }
    }
}

#pragma warning restore CA2000