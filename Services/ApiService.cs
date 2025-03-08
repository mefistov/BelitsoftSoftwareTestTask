using RestSharp;
using System.Text.Json;
using System.Net;
using BelitsoftSoftwareTestTask.Models;
using BelitsoftSoftwareTestTask.Config;
using NUnit.Framework;
using FluentAssertions.Equivalency.Steps;
using System.Dynamic;

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

    
        // Recursive dynamic parser using ExpandoObject for objects and List<dynamic> for arrays.
        private dynamic ParseJsonElementToDynamic(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    IDictionary<string, object> expando = new ExpandoObject();
                    foreach (var property in element.EnumerateObject())
                    {
                        expando[property.Name] = ParseJsonElementToDynamic(property.Value);
                    }
                    return expando;
                case JsonValueKind.Array:
                    var list = new List<dynamic>();
                    foreach (var item in element.EnumerateArray())
                    {
                        list.Add(ParseJsonElementToDynamic(item));
                    }
                    return list;
                case JsonValueKind.String:
                    return element.GetString();
                case JsonValueKind.Number:
                    if (element.TryGetInt32(out int intVal))
                        return intVal;
                    return element.GetDouble();
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return element.GetBoolean();
                default:
                    return null;
            }
        }

        public async Task<GetLocationResponce<dynamic>> getShipList(Dictionary<string, string> queryParams)
        {
            try
            {
                var response = await getSearchCruises(queryParams);
                var statusCode = (HttpStatusCode)response.StatusCode;
                var content = response.Content;
                
                if (statusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(content))
                {
                    var jsonDocument = JsonDocument.Parse(content);
                    var root = jsonDocument.RootElement;

                    if (root.TryGetProperty("data", out JsonElement dataElement))
                    {
                        dynamic dynamicData = ParseJsonElementToDynamic(dataElement);
                        return new GetLocationResponce<dynamic>(true, ((int)statusCode).ToString(), dynamicData, null );
                    }
                    return new GetLocationResponce<dynamic>( false, ((int)statusCode).ToString(), null, "Data property not found in response"
                    );
                }
                return new GetLocationResponce<dynamic>( false, ((int)statusCode).ToString(), null, response.ErrorMessage ?? "Invalid response" );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in getListList2: {ex.Message}");
                return new GetLocationResponce<dynamic>( false, "500", null,  ex.Message );
            }
        }

        public async Task<GetLocationResponce<List<Cruise>>> getCruisesLocationList()
        {
            try
            {
                var response = await getCruisesLocation();
                var statusCode = (HttpStatusCode)response.StatusCode;
                var content = response.Content;

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
