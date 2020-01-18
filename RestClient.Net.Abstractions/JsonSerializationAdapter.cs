using RestClient.Net.Abstractions;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RestClient.Net
{
    public class JsonSerializationAdapter : ISerializationAdapter
    {
        #region Implementation
        public async Task<TResponseBody> Deserialize<TResponseBody>(Stream data, IHeadersCollection responseHeaders)
        {
            var returnValue = await JsonSerializer.DeserializeAsync<TResponseBody>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).AsTask();

            return returnValue;
        }

        public Task<byte[]> Serialize<TRequestBody>(TRequestBody value, IHeadersCollection requestHeaders)
        {
            var json = JsonSerializer.Serialize(value, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            //This here is why I don't like JSON serialization. 😢
            var binary = Encoding.UTF8.GetBytes(json);

            return Task.FromResult(binary);
        }
        #endregion
    }
}

