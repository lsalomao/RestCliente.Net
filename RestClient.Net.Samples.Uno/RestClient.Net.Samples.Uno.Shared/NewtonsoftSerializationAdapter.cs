using Newtonsoft.Json;
using RestClient.Net.Abstractions;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Net
{
    public class NewtonsoftSerializationAdapter : ISerializationAdapter
    {
        private JsonSerializer JsonSerializer = JsonSerializer.Create();

        #region Implementation
        public TResponseBody Deserialize<TResponseBody>(byte[] data, IHeadersCollection responseHeaders) where TResponseBody : class
        {

            using (var stream = new MemoryStream(data))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var responseBody = JsonSerializer.Deserialize(reader, typeof(TResponseBody));
                return responseBody as TResponseBody;
            }
        }

        public byte[] Serialize<TRequestBody>(TRequestBody value, IHeadersCollection requestHeaders)
        {
            var json = JsonConvert.SerializeObject(value);

            //This here is why I don't like JSON serialization. 😢
            var binary = Encoding.UTF8.GetBytes(json);

            return binary;
        }
        #endregion
    }
}
