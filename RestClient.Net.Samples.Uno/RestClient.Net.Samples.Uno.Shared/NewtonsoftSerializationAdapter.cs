using Newtonsoft.Json;
using RestClient.Net.Abstractions;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Net
{
    public class NewtonsoftSerializationAdapter : ISerializationAdapter
    {
        JsonSerializer JsonSerializer = JsonSerializer.Create();

        #region Implementation
        public TResponseBody Deserialize<TResponseBody>(Stream data, IHeadersCollection responseHeaders)
        {
            using (var reader = new StreamReader(data, Encoding.UTF8))
            {
                var responseBody = (TResponseBody)JsonSerializer.Deserialize(reader, typeof(TResponseBody));
                return responseBody;
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
