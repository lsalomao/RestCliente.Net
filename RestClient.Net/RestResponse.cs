using RestClientDotNet.Abstractions;
using System;
using System.Net.Http;

namespace RestClientDotNet
{
    public class RestResponse<TResponseBody> : RestResponseBase<TResponseBody>
    {
        #region Public Properties
        public Uri RequestUri { get; }
        public HttpResponseMessage HttpResponseMessage { get; }
        public override bool IsSuccess => HttpResponseMessage.IsSuccessStatusCode;
        #endregion

        #region Constructor
        public RestResponse
        (
            IRestHeaders restHeadersCollection,
            int statusCode,
            Uri requestUri,
            HttpVerb httpVerb,
            byte[] responseContentData,
            TResponseBody body,
            HttpResponseMessage httpResponseMessage
            ) : base(restHeadersCollection, statusCode, httpVerb, responseContentData, body)
        {
            RequestUri = requestUri;
            HttpResponseMessage = httpResponseMessage;

        }
        #endregion
    }
}
