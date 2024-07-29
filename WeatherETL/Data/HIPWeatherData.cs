using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherETL.Data
{
    public class HIPWeatherData
    {
        public string? ForecastDate { get; set; }

        public string? Week { get; set; }

        public string? ForecastWind { get; set; }

        public string? ForecastWeather { get; set; }

        public HIPValueUnitItem? ForecastMaxtemp { get; set; }

        public HIPValueUnitItem? ForecastMintemp { get; set; }

        public HIPValueUnitItem? ForecastMaxrh { get; set; }

        public HIPValueUnitItem? ForecastMinrh { get; set; }

        public int ForecastIcon { get; set; }

        public string? PSR { get; set; }
    }

    public class HIPValueUnitItem
    {
        public double Value { get; set; }

        public string? Unit { get; set; }

    }
}
