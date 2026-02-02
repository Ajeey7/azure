using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Net.Http;
using Xunit;

namespace MyDotNetAppLocal.Tests
{
    public class WeatherControllerTests
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly WeatherController _controller;

        public WeatherControllerTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockConfiguration = new Mock<IConfiguration>();
            
            // Setup mock configuration
            _mockConfiguration.Setup(c => c["OpenWeatherMap:ApiKey"]).Returns("");
            
            _controller = new WeatherController(_mockHttpClientFactory.Object, _mockConfiguration.Object);
        }

        [Fact]
        public void GetCurrentWeather_WithLocation_ReturnsMockData()
        {
            // Act
            var result = _controller.GetCurrentWeather("Stockholm");
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public void GetForecast_WithLocation_ReturnsMockForecast()
        {
            // Act
            var result = _controller.GetForecast("Stockholm");
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public void GetCurrentWeather_WithEmptyLocation_ReturnsDefaultLocation()
        {
            // Act
            var result = _controller.GetCurrentWeather("");
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }
    }
}
