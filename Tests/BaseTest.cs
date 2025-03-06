using NUnit.Framework;
using FluentAssertions;
using BelitsoftSoftwareTestTask.Services;
using NUnit.Framework.Internal;

namespace BelitsoftSoftwareTestTask.Tests
{
    public class BaseTest
    {
        private ApiService? _apiService;

        [SetUp]
        public void Setup()
        {
            _apiService = new ApiService();
        }

        [Test]
        public async Task GetSomeDataAsync_ShouldReturnSuccess()
        {
            if (_apiService != null)
            {
                var response = await _apiService.GetSomeDataAsync();
                
                TestContext.WriteLine($"API Response - IsSuccessful: {response.IsSuccessful}");
                if (!response.IsSuccessful)
                {
                    TestContext.WriteLine($"Error Message: {response.ErrorMessage ?? "Unknown error"}");
                }

                response.IsSuccessful.Should().BeTrue();
                TestContext.WriteLine($"API Response - Data: {response.Data?.ToString()}");
                response.Data.Should().NotBeNull();
            }
        }
    }
}