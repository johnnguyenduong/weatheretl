using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherETL.Data;
using WeatherETL.Services;
using WeatherETL.Utils;

namespace WeatherETL.ETL
{
    public class WeatherETLProcess : ETLBaseProcess<HKWeatherData, HIPWeatherData>
    {

        private readonly IWeatherService _weatherService;
        private readonly ICsvService _csvService;
        private readonly IBlobStorageService _azureStorageService;
        private readonly ILogger<WeatherETLProcess> _logger;

        public WeatherETLProcess(
            IWeatherService weatherService,
            ICsvService csvService,
            IBlobStorageService azureStorageService,
            ILogger<WeatherETLProcess> logger
        ) 
        {
            _weatherService = weatherService;
            _csvService = csvService;
            _azureStorageService = azureStorageService;
            _logger = logger;
        }

        protected override async Task<IEnumerable<HKWeatherData>> ExtractAsync()
        {
            return await _weatherService.GetWeatherDataAsync();
        }

        protected override IEnumerable<HIPWeatherData> Transform(IEnumerable<HKWeatherData> weatherDataLst)
        {
            var hipWeatherDataLst = new List<HIPWeatherData>();

            foreach (var weatherData in weatherDataLst)
            {
                var hipData = new HIPWeatherData
                {
                    ForecastDate = weatherData.ForecastDate,
                    ForecastIcon = weatherData.ForecastIcon,
                    ForecastMaxrh = CloneItemWithConvertCelToFah(weatherData.ForecastMaxrh),
                    ForecastMaxtemp = CloneItemWithConvertCelToFah(weatherData.ForecastMaxtemp),
                    ForecastMinrh = CloneItemWithConvertCelToFah(weatherData.ForecastMinrh),
                    ForecastMintemp = CloneItemWithConvertCelToFah(weatherData.ForecastMintemp),
                    ForecastWeather = weatherData.ForecastWeather,
                    ForecastWind = weatherData.ForecastWind,
                    PSR = weatherData.PSR,
                    Week = weatherData.Week
                };

                hipWeatherDataLst.Add(hipData);
            }

            return hipWeatherDataLst;
        }

        protected override async Task LoadAsync(IEnumerable<HIPWeatherData> weatherDataLst)
        {
            string csvFilePath = $"weather_data{DateTime.Now.ToString("yyyyddMHHmmss")}.csv";
            await _csvService.SaveToCsvAsync(weatherDataLst, csvFilePath);

            // upload to azure
            await _azureStorageService.UploadToBlobAsync(csvFilePath);
        }

        protected override void OnFailed(Exception ex) 
        {
            _logger.LogError(ex, "An error occurred while gathering and processing weather data.");
        }

        protected override void OnSucceeded() 
        {
            _logger.LogInformation("Process weather data successfully.");
        }

        private HIPValueUnitItem? CloneItemWithConvertCelToFah(ValueUnitItem? item) 
        {
            if (item == null)
                return null;
            double value = item.Value;
            string? unit = item.Unit;
            if ("C".Equals(unit)) 
            {
                value = TemperatureConverter.CelsiusToFahrenheit(value);
                unit = "F";
            }

            return new HIPValueUnitItem
            {
                Value = value,
                Unit = unit
            };
        }
    }
   
}
