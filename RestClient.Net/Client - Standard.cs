using System;

namespace RestClient.Net
{
    public sealed partial class Client
    {
#if NETCOREAPP3_0 || NETSTANDARD2_0
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
#endif
    }
}
