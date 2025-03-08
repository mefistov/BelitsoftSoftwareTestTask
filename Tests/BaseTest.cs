using NUnit.Framework;
using FluentAssertions;
using BelitsoftSoftwareTestTask.Services;
using NUnit.Framework.Internal;
using BelitsoftSoftwareTestTask.Models;

namespace BelitsoftSoftwareTestTask.Tests
{
    [TestFixture]
    public class BaseTest
    {
        private List<Cruise> filteredCruise = new List<Cruise>();
        private int selectedDestinationID;

        
        private ApiService? apiService;

        [SetUp]
        public void Setup()
        {
            apiService = new ApiService();
        }

    [Test, Order(1)]
        public async Task getCruisesLocation()
        {
            if (apiService != null)
            {
                var response = await apiService.getCruisesLocationList();
                
                TestContext.WriteLine($"API Response - IsSuccessful: {response.IsSuccessful}");
                TestContext.WriteLine($"API Response - Status: {response.Status}");
                TestContext.WriteLine($"API Response - Status: {response}");
                
                if (!response.IsSuccessful)
                {
                    TestContext.WriteLine($"Error Message: {response.ErrorMessage ?? "Unknown error"}");
                }

                TestContext.WriteLine($"Selected cruise with caribean destination: {selectedDestinationID}");
                response.IsSuccessful.Should().BeTrue();
                response.Data.Should().NotBeNull();

                filteredCruise = response.Data.Where(cruise => cruise.Name.Equals(Destination.caribean)).ToList();
                selectedDestinationID = filteredCruise.First().DestinationId;
                TestContext.WriteLine($"Selected cruise with caribean destination: {selectedDestinationID}");
            }
        }

        [Test, Order(2)]
        public async Task getGETSearchCruises()
        {
            if (apiService != null)
            {
                selectedDestinationID.Should().NotBe(0);
                var queryParams = new Dictionary<string, string>
                {
                    { "destinationId", selectedDestinationID.ToString() },
                    };
                    var response = await apiService.getShipList(queryParams);
                    response.Should().NotBeNull();
                    TestContext.WriteLine($"API Response - Status: {response.Status}");
                    
                    if (response.IsSuccessful && response.Data != null)
                    {
                        dynamic dynamicData = response.Data;
                        var sortedShips = ExtractSortedShips(dynamicData);
                        
                        foreach (var ship in sortedShips)
                        {
                            var id = ship.id;
                            var shipName = ship.name;
                            var crew = ship.crew;
                            TestContext.WriteLine($"Ship: {shipName} (ID: {id}) - Crew: {crew}");
                            }
                    }
                    else
                    {
                        TestContext.WriteLine($"API request failed. Error: {response.ErrorMessage}");
                        response.Status.Should().NotBe("500", "API should not return a 500 error");
                        }
            }
}

        private List<dynamic> ExtractSortedShips(dynamic dynamicData)
        {
            var shipList = new List<dynamic>();
            
            if (!((IDictionary<string, object>)dynamicData).ContainsKey("list"))
                return shipList;
            
            var cruiseList = dynamicData.list as IEnumerable<dynamic>;
            if (cruiseList != null)
            {
                foreach (var item in cruiseList)
                {
                    if (item is IDictionary<string, object> dict && dict.ContainsKey("ship") && item.ship != null)
                    {
                        shipList.Add(item.ship);
                    }
                }
            }
            
            var sortedShips = shipList.OrderByDescending(ship => ship.crew != null ? (int)ship.crew : 0).ToList();
            return sortedShips;
        }
    }
        
}