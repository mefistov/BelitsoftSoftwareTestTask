using NUnit.Framework;
using FluentAssertions;
using BelitsoftSoftwareTestTask.Services;
using NUnit.Framework.Internal;

namespace BelitsoftSoftwareTestTask.Tests
{
    public class BaseTest
    {
        private ApiService? apiService;

        [SetUp]
        public void Setup()
        {
            apiService = new ApiService();
        }

        [Test]
        public async Task GetSomeDataAsync_ShouldReturnSuccess()
        {
            if (apiService != null)
            {
                var response = await apiService.GetResponse();
                
                TestContext.WriteLine($"API Response - IsSuccessful: {response.IsSuccessful}");
                TestContext.WriteLine($"API Response - Status: {response.Status}");
                TestContext.WriteLine($"API Response - Status: {response}");
                
                if (!response.IsSuccessful)
                {
                    TestContext.WriteLine($"Error Message: {response.ErrorMessage ?? "Unknown error"}");
                }

                response.IsSuccessful.Should().BeTrue();
                TestContext.WriteLine($"API Response - Data: {response.Data.FirstOrDefault()?.DestinationId}");
                response.Data.Should().NotBeNull();
            }
        }
    }
}