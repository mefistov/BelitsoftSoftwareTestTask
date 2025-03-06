using NUnit.Framework;
using FluentAssertions;
using BelitsoftSoftwareTestTask.Services;
using System.Threading.Tasks;

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
                response.IsSuccessful.Should().BeTrue();
            }
        }
    }
}