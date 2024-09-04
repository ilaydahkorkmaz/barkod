using Newtonsoft.Json;
using YourNamespace.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace YourNamespace.Services
{
    public class TetkikService
    {
        private readonly HttpClient _httpClient;

        public TetkikService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Root> GetTetkikDataAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "domain(st24tevar)");
            request.Headers.Add("api-key", "keypublicrandom");
            request.Headers.Add("Cookie", "frontend_lang=tr_TR; session_id=xx(st24tevar)");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Root>(jsonResponse);
        }
    }
}
