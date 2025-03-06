using RestSharp;
using System.Text.Json;
using BelitsoftSoftwareTestTask.Models;
using BelitsoftSoftwareTestTask.Config;
using NUnit.Framework;

namespace BelitsoftSoftwareTestTask.Services
{
    public class ApiService
    {
        private readonly string _apiKey;
        private readonly string _apiHost;
        private readonly string _baseUrl;

        public ApiService()
        {
            _apiKey = ApiConfig.ApiKey;
            _apiHost = ApiConfig.ApiHost;
            _baseUrl = ApiConfig.BaseUrl;
        }

        private RestClient CreateClient()
        {
            return new RestClient(_baseUrl);
        }

        private RestRequest CreateRequest(string endpoint, Method method)
        {
            var request = new RestRequest(endpoint, method);
            request.AddHeader("x-rapidapi-key", _apiKey);
            request.AddHeader("x-rapidapi-host", _apiHost);
            return request;
        }

        public async Task<ApiResponse<List<Destination>>> GetSomeDataAsync()
        {
            try
            {
                var data = await LoadDestinationsDataAsync();
                return new ApiResponse<List<Destination>>
                {
                    IsSuccessful = true,
                    Data = data ?? new List<Destination>()
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Destination>>
                {
                    IsSuccessful = false,
                    Data = new List<Destination>(),
                    ErrorMessage = ex.Message
                };
            }
        }

        private async Task<List<Destination>> LoadDestinationsDataAsync()
        {
            try
            {
                var response = await GetCruisesLocation();
                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var jsonDocument = JsonDocument.Parse(response.Content);
                    var locations = jsonDocument.RootElement.GetProperty("data").EnumerateArray();
                    
                    var destinations = new List<Destination>();
                    foreach (var location in locations)
                    {
                        destinations.Add(new Destination
                        {
                            LocationId = location.GetProperty("location_id").GetInt32(),
                            Name = location.GetProperty("name").GetString(),
                            DestinationId = destinations.Count + 1
                        });
                    }
                    return destinations;
                }
                return new List<Destination>();
            }
            catch
            {
                return new List<Destination>();
            }
        }

        private async Task<RestResponse> GetCruisesLocation()
        {
            var client = CreateClient();
            var request = CreateRequest(Endpoints.GetCruisesLocation, Method.Get);
            TestContext.WriteLine($"Requesting URL: {client.BuildUri(request)}");
            return await client.ExecuteAsync(request);
        }
    }
}
