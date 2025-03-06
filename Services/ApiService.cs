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

        private RestRequest CreateGetRequest(string endpoint, Method method)
        {
            var request = new RestRequest(endpoint, method);
            request.AddHeader("x-rapidapi-key", _apiKey);
            request.AddHeader("x-rapidapi-host", _apiHost);
            return request;
        }

        private async Task<RestResponse> GetCruisesLocation()
        {
            var client = CreateClient();
            var request = CreateGetRequest(Endpoints.GetCruisesLocation, Method.Get);
            TestContext.WriteLine($"Requesting URL: {client.BuildUri(request)}");
            return await client.ExecuteAsync(request);
        }

        public async Task<ApiResponse<List<Cruise>>> GetResponse()
        {
            try
            {
                var response = await GetCruisesLocation();
                var statusCode = (HttpStatusCodeEnum)response.StatusCode;
                var content = response.Content;
                Console.WriteLine(content);

                if (statusCode == HttpStatusCodeEnum.OK && !string.IsNullOrEmpty(content))
                {
                    var cruises = new List<Cruise>();
                    var jsonDocument = JsonDocument.Parse(content);
                    var root = jsonDocument.RootElement;

                    if (root.TryGetProperty("data", out JsonElement dataElement))
                    {
                        foreach (var element in dataElement.EnumerateArray())
                        {
                            try
                            {
                                var cruise = new Cruise
                                {
                                    DestinationId = element.TryGetProperty("destinationId", out var destId) ? destId.GetInt32() : 0,
                                    LocationId = element.TryGetProperty("locationId", out var locId) ? locId.GetInt32() : 0,
                                    Name = element.TryGetProperty("name", out var name) ? name.GetString() : string.Empty
                                };
                                cruises.Add(cruise);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error parsing cruise element: {ex.Message}");
                                continue;
                            }
                        }
                        return new ApiResponse<List<Cruise>>(true, ((int)statusCode).ToString(), cruises, null);
                    }
                    return new ApiResponse<List<Cruise>>(false, ((int)statusCode).ToString(), null, "Data property not found in response");
                }
                return new ApiResponse<List<Cruise>>(false, ((int)statusCode).ToString(), null, response.ErrorMessage);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Cruise>>(false, "500", null, ex.Message);
            }
        }
    }
}
