using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using WeatherETL.Data;
using WeatherETL.Utils;

namespace WeatherETL.Services
{
    public interface IWeatherService
    {
        Task<List<HKWeatherData>> GetWeatherDataAsync();
    }
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _apiUrl;
        private readonly ILogger<WeatherService> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly int _retryInterval;

        public WeatherService(IConfiguration configuration, HttpClient httpClient, ILogger<WeatherService> logger)
        {
            _httpClient = httpClient;
            _apiUrl = configuration["HKWeatherUrl"];
            if (_apiUrl == null) 
            {
                throw new ArgumentNullException("HKWeatherUrl");
            }
            Int32.TryParse(configuration["NumberRetry"], out _retryInterval);
            _logger = logger;

            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(_retryInterval, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning($"Retry {retryCount} due to {exception.Message}");
                    });
        }

        public async Task<List<HKWeatherData>> GetWeatherDataAsync()
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                _logger.LogInformation($"Trying download data from {_apiUrl} ....");
                var response = await _httpClient.GetStringAsync(_apiUrl);
                var weatherData = HipJsonSerializer.Deserialize<HKWeatherForeCast>(response);
                if (weatherData == null)
                    throw new Exception($"Could not download data from {_apiUrl}");
                _logger.LogInformation($"Finished download data from {_apiUrl}");
                return weatherData.WeatherDataList;

            });
        }
    }
}
