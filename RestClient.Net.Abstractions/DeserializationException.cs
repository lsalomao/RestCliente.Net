using System;

namespace RestClientDotNet.Abstractions
{
    public class DeserializationException : Exception
    {
        private readonly byte[] _responseData;

        public DeserializationException(
            string message,
            byte[] responseData,
            Exception innerException) : base(message, innerException)
        {
            _responseData = responseData;
        }

        public byte[] GetResponseData()
        {
            return _responseData;
        }
    }
}
