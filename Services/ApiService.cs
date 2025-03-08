using RestSharp;
using System.Text.Json;
using BelitsoftSoftwareTestTask.Models;
using BelitsoftSoftwareTestTask.Config;
using NUnit.Framework;
using FluentAssertions.Equivalency.Steps;

namespace BelitsoftSoftwareTestTask.Services
{
    public class ApiService
    {
        private readonly string apiKey;
        private readonly string apiHost;
        private readonly string baseUrl;

        public ApiService()
        {
            apiKey = ApiConfig.ApiKey;
            apiHost = ApiConfig.ApiHost;
            baseUrl = ApiConfig.BaseUrl;
        }

        private RestClient createClient()
        {
            return new RestClient(baseUrl);
        }

        private RestRequest createGetRequest(string endpoint, Method method)
        {
            var request = new RestRequest(endpoint, method);
            request.AddHeader("x-rapidapi-key", apiKey);
            request.AddHeader("x-rapidapi-host", apiHost);
            return request;
        }

        private async Task<RestResponse> getCruisesLocation()
        {
            var client = createClient();
            var request = createGetRequest(Endpoints.GetCruisesLocation, Method.Get);
            TestContext.WriteLine($"Requesting URL: {client.BuildUri(request)}");
            return await client.ExecuteAsync(request);
        }

        private async Task<RestResponse> getSearchCruises(Dictionary<string, string> queryParams)
        {
            var client = createClient();
            var urlWithParams = getSearchCruisesUrlWithParams(queryParams);
            var request = createGetRequest(urlWithParams, Method.Get);
            TestContext.WriteLine($"Requesting URL: {client.BuildUri(request)}");
            return await client.ExecuteAsync(request);
        }

         public async Task<GetLocationResponce<List<Ships<Ship>>>> getListList(Dictionary<string, string> queryParams)
        {
            try
            {   
                var response = await getSearchCruises(queryParams);
                var statusCode = (HttpStatusCode)response.StatusCode;
                var content = response.Content;
                Console.WriteLine(content);

                if (statusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(content))
                {
                    var ships = new List<Ships<Ship>>();
                    var jsonDocument = JsonDocument.Parse(content);
                    var root = jsonDocument.RootElement;

                    if (root.TryGetProperty("data", out JsonElement dataElement))
                    {
                        foreach (var element in dataElement.EnumerateArray())
                        {
                            try
                            {
                                var shipId = element.TryGetProperty("shipId", out var shipIdElement) ? shipIdElement.GetString() : string.Empty;
                                var seoName = element.TryGetProperty("seoName", out var seoNameElement) ? seoNameElement.GetString() : string.Empty;
                                var length = element.TryGetProperty("length", out var lengthElement) ? lengthElement.GetInt32() : 0;
                                var id = element.TryGetProperty("id", out var idElement) ? idElement.GetInt32() : 0;
                                var ship = new Ships<Ship>(shipId, seoName, string.Empty, length, id, new List<Ship>())
                                {
                                    Ship = new List<Ship>()
                                };

                                if (element.TryGetProperty("ship", out JsonElement shipElement))
                                {
                                    foreach (var shipElementItem in shipElement.EnumerateArray())
                                    {
                                        var innerShipId = shipElementItem.TryGetProperty("shipId", out var shipIdValue) ? shipIdValue.GetString() ?? string.Empty : string.Empty;
                                        var name = shipElementItem.TryGetProperty("name", out var nameValue) ? nameValue.GetString() ?? string.Empty : string.Empty;
                                        var crew = shipElementItem.TryGetProperty("crew", out var crewValue) ? crewValue.GetInt32() : 0;
                                        var shipItem = new Ship(innerShipId, name, crew);
                                        ship.Ship.Add(shipItem);
                                    }
                                }
                                ships.Add(ship);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error parsing ship element: {ex.Message}");
                                continue;
                            }
                        }
                        return new GetLocationResponce<List<Ships<Ship>>>(true, ((int)statusCode).ToString(), ships, null);
                    }
                    return new GetLocationResponce<List<Ships<Ship>>>(false, ((int)statusCode).ToString(), new List<Ships<Ship>>(), "Data property not found in response");
                }
                return new GetLocationResponce<List<Ships<Ship>>>(false, ((int)statusCode).ToString(), new List<Ships<Ship>>(), "");
            }
            catch (Exception ex)
            {
                return new GetLocationResponce<List<Ships<Ship>>>(false, "500", new List<Ships<Ship>>(), ex.Message);
            }
        }

        public async Task<GetLocationResponce<List<Cruise>>> getCruisesLocationList()
        {
            try
            {
                var response = await getCruisesLocation();
                var statusCode = (HttpStatusCode)response.StatusCode;
                var content = response.Content;
                Console.WriteLine(content);

                if (statusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(content))
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
                        return new GetLocationResponce<List<Cruise>>(true, ((int)statusCode).ToString(), cruises, null);
                    }
                    return new GetLocationResponce<List<Cruise>>(false, ((int)statusCode).ToString(), null, "Data property not found in response");
                }
                return new GetLocationResponce<List<Cruise>>(false, ((int)statusCode).ToString(), null, response.ErrorMessage);
            }
            catch (Exception ex)
            {
                return new GetLocationResponce<List<Cruise>>(false, "500", null, ex.Message);
            }
        } 

        public string getSearchCruisesUrlWithParams(Dictionary<string, string> queryParams)
        {
            var client = createClient();
            var request = new RestRequest(Endpoints.SearchCruises, Method.Get);

            foreach (var param in queryParams)
            {
                request.AddQueryParameter(param.Key, param.Value);
            }

            var url = client.BuildUri(request).ToString();
            return url;
        }

        public async Task<GetLocationResponce<List<Cruise>>> getSearchCruisesList()
        {
            try
            {
                var response = await getCruisesLocation();
                var statusCode = (HttpStatusCode)response.StatusCode;
                var content = response.Content;
                Console.WriteLine(content);

                if (statusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(content))
                {
                    var cruises = new List<Cruise>();
                    var jsonDocument = JsonDocument.Parse(content);
                    var root = jsonDocument.RootElement;
                    return new GetLocationResponce<List<Cruise>>(true, ((int)statusCode).ToString(), cruises, null);
                }
                return new GetLocationResponce<List<Cruise>>(false, ((int)statusCode).ToString(), null, "Invalid response");
            }
            catch (Exception ex)
            {
                return new GetLocationResponce<List<Cruise>>(false, "500", null, ex.Message);
            }
        } 
    }
}
