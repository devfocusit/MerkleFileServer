using System.Net;
using System.Text.Json;

namespace MerkleFileServer.Addon.AsClient.ApiClient
{
    public static class HttpClientExtension
    {
        public static async Task<T?> Get<T>(this HttpClient client, Uri uri)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            if ((object)uri == null)
            {
                throw new ArgumentNullException("resource");
            }

            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            var response = await client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception("BadRequest");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Unable to communicate successfully with API");
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return default;
            }

            using var contentStream = await response.Content.ReadAsStreamAsync();

            var result = JsonSerializer.Deserialize<T>(contentStream);

            return result;
        }
    }
}
