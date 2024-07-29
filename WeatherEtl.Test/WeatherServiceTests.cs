using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Threading;
using Xunit;
using System.Text.Json;
using WeatherETL.Services;
using WeatherETL.Data;
using WeatherETL.Utils;
using Newtonsoft.Json;

namespace WeatherEtl.Test
{
    public class WeatherServiceTests
    {
        private readonly Mock<ILogger<WeatherService>> _loggerMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;

        public WeatherServiceTests()
        {
            _loggerMock = new Mock<ILogger<WeatherService>>();
            _configMock = new Mock<IConfiguration>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        }

        [Fact]
        public async Task GetWeatherDataAsync_ReturnsForecasts_WhenApiCallSucceeds()
        {
            // Arrange
            var expectedForecasts = new List<HKWeatherData>();
            expectedForecasts.Add(
               new HKWeatherData
               {
                   ForecastDate = "20240730",
                   ForecastIcon = 64,
                   ForecastMaxrh = new ValueUnitItem { Value = 95, Unit = "percent" },
                   ForecastMinrh = new ValueUnitItem { Value = 80, Unit = "percent" },
                   ForecastMaxtemp = new ValueUnitItem { Value = 29, Unit = "C" },
                   ForecastMintemp = new ValueUnitItem { Value = 26, Unit = "C" },
                   ForecastWeather = "Mainly cloudy with occasional showers and a few squally thunderstorms. Showers will be heavy at times",
                   ForecastWind = "South to southeast force 4, occasionally force 5 offshore.",
                   PSR = "High",
                   Week = "Tuesday"
               }
            );

            var hkWeatherCast = new HKWeatherForeCast()
            {
                WeatherDataList = expectedForecasts

            };

            _configMock.Setup(c => c["HKWeatherUrl"]).Returns("http://test-api.com");
            _configMock.Setup(c => c["NumberRetry"]).Returns("3");

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(HipJsonSerializer.Serialize(hkWeatherCast))
                });

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            var weatherService = new WeatherService(_configMock.Object, httpClient, _loggerMock.Object);

            // Act
            var result = await weatherService.GetWeatherDataAsync();

            // Assert
            Assert.Equal(expectedForecasts.Count, result.Count);
            Assert.Equal(expectedForecasts[0].ForecastWind, result[0].ForecastWind);
            Assert.Equal(expectedForecasts[0].ForecastMaxtemp?.Value, result[0].ForecastMaxtemp?.Value);
        }

        [Fact]
        public async Task GetWeatherDataAsync_RetriesAndThrows_WhenApiCallFails()
        {
            // Arrange
            _configMock.Setup(c => c["HKWeatherUrl"]).Returns("http://test-api.com");
            _configMock.Setup(c => c["NumberRetry"]).Returns("3");


            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            var weatherService = new WeatherService(_configMock.Object, httpClient, _loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => weatherService.GetWeatherDataAsync());

            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(4),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
