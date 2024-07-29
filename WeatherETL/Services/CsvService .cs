using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherETL.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WeatherETL.Services
{
    public interface ICsvService
    {
        Task SaveToCsvAsync(IEnumerable<HIPWeatherData> data, string filePath);
    }
    public class CsvService : ICsvService
    {
        private const string DELIMITER = ";";
        private readonly ILogger<CsvService> _logger;

        public CsvService(ILogger<CsvService> logger) 
        {
            _logger = logger;
        }

        public async Task SaveToCsvAsync(IEnumerable<HIPWeatherData> weatherData, string filePath)
        {
            _logger.LogInformation($"Saving data to {filePath}...");
            var csvLines = new List<string>
            {
                $"ForecastDate{DELIMITER}Week{DELIMITER}ForecastWind{DELIMITER}ForecastWeather{DELIMITER}ForecastMaxtemp{DELIMITER}ForecastMintemp{DELIMITER}ForecastMaxrh{DELIMITER}ForecastMinrh{DELIMITER}ForecastIcon{DELIMITER}PSR"
            };

            csvLines.AddRange(weatherData.Select(data =>
                $"{data.ForecastDate}{DELIMITER}{data.Week}{DELIMITER}" +
                $"{data.ForecastWind}{DELIMITER}{data.ForecastWeather}{DELIMITER}" +
                $"{data.ForecastMaxtemp?.Value} {data.ForecastMaxtemp?.Unit}{DELIMITER}" +
                $"{data.ForecastMintemp?.Value} {data.ForecastMintemp?.Unit}{DELIMITER}" +
                $"{data.ForecastMaxrh?.Value} {data.ForecastMaxrh?.Unit}{DELIMITER}" +
                $"{data.ForecastMinrh?.Value} {data.ForecastMinrh?.Unit}{DELIMITER}" +
                $"{data.ForecastIcon}{DELIMITER}{data.PSR}"
            ));

            //var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            //{
            //    Delimiter = DELIMITER
            //};

            //await using var writer = new StreamWriter(filePath);
            //await using var csv = new CsvWriter(writer, config);
            //await csv.WriteRecordsAsync(weatherData);

            await File.WriteAllLinesAsync(filePath, csvLines);

            _logger.LogInformation($"Saved data to {filePath} successfully.");
        }
    }
}
