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
                { "order", "popularity" },
                { "page", "1" },
                { "currencyCode", "USD" }
            };
            var response = await apiService.getListList(queryParams);
            response.Should().NotBeNull();
            TestContext.WriteLine($"API Response - Status: {response.Status}");

         //response.Data.OrderBy(ship => ship.Name).ToList();
         var sortedShips = response.Data.Where(List => List.Ship != null).SelectMany(List => List.Ship).OrderByDescending(ship => ship.Crew).ToList();
         sortedShips.Should().NotBeNull();
         TestContext.WriteLine($"Ships count: {sortedShips.Count}");
            foreach (var ship in sortedShips)
            {
            TestContext.WriteLine($"Ship: {ship.Name} - Crew: {ship.Crew}");
            }
            }
        }
    }
}