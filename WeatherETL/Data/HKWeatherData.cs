using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WeatherETL.Data
{
    public class HKWeatherData
    {
        [JsonProperty("forecastDate")]
        public string? ForecastDate { get; set; }

        [JsonProperty("week")]
        public string? Week { get; set; }

        [JsonProperty("forecastWind")]
        public string? ForecastWind { get; set; }

        [JsonProperty("forecastWeather")]
        public string? ForecastWeather { get; set; }

        [JsonProperty("forecastMaxtemp")]
        public ValueUnitItem? ForecastMaxtemp { get; set; }

        [JsonProperty("forecastMintemp")]
        public ValueUnitItem? ForecastMintemp { get; set; }

        [JsonProperty("forecastMaxrh")]
        public ValueUnitItem? ForecastMaxrh { get; set; }

        [JsonProperty("forecastMinrh")]
        public ValueUnitItem? ForecastMinrh { get; set; }

        [JsonProperty("ForecastIcon")]
        public int ForecastIcon { get; set; }

        [JsonProperty("PSR")]
        public string? PSR { get; set; }
    }

    public class ValueUnitItem
    {
        [JsonProperty("value")]
        public double Value { get; set; }

        [JsonProperty("unit")]
        public string? Unit { get; set; }
    }
    public class HKWeatherForeCast
    {
        [JsonProperty("weatherForecast")]
        public required List<HKWeatherData> WeatherDataList{ get; set; }
    }
}
