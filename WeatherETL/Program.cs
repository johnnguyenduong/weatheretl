using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherETL.ETL;
using WeatherETL.Services;

namespace WeatherETL
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration((hostingContext, config) =>
                 {
                     config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                     config.AddEnvironmentVariables();
                 })
                 .ConfigureServices((hostContext, services) =>
                 {
                     services.AddHttpClient();
                     services.AddSingleton<IWeatherService, WeatherService>();
                     services.AddSingleton<ICsvService, CsvService>();
                     services.AddSingleton<IBlobStorageService, BlobStorageService>();
                     services.AddSingleton<WeatherETLProcess>();
                 })
                 .Build();

            var weatherEtl = host.Services.GetRequiredService<WeatherETLProcess>();
            await weatherEtl.Process();
        }
    }
}