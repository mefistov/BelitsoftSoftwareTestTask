using RestSharp;
using System.Threading.Tasks;

namespace BelitsoftSoftwareTestTask.Services
{
    public class ApiService
    {
        private readonly RestClient _client;

        public ApiService()
        {
            _client = new RestClient("https://tripadvisor16.p.rapidapi.com");
        }

        public async Task<RestResponse> GetSomeDataAsync()
        {
            var request = new RestRequest("/endpoint", Method.Get);
            request.AddHeader("X-RapidAPI-Key", "YOUR_RAPIDAPI_KEY");
            request.AddHeader("X-RapidAPI-Host", "tripadvisor16.p.rapidapi.com");

            return await _client.ExecuteAsync(request);
        }
    }
}