using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyDotNetAppLocal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        
        // För demo använder vi mock-data, men du kan enkelt byta till riktig API
        private readonly string _apiKey;
        private readonly bool _useRealApi;

        public WeatherController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _apiKey = _configuration["OpenWeatherMap:ApiKey"] ?? "";
            _useRealApi = !string.IsNullOrEmpty(_apiKey);
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentWeather([FromQuery] string location = "Stockholm")
        {
            try
            {
                if (_useRealApi)
                {
                    return await GetRealCurrentWeather(location);
                }
                else
                {
                    return GetMockCurrentWeather(location);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Kunde inte hämta väderdata: {ex.Message}" });
            }
        }

        [HttpGet("forecast")]
        public async Task<IActionResult> GetForecast([FromQuery] string location = "Stockholm")
        {
            try
            {
                if (_useRealApi)
                {
                    return await GetRealForecast(location);
                }
                else
                {
                    return GetMockForecast(location);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Kunde inte hämta prognos: {ex.Message}" });
            }
        }

        private async Task<IActionResult> GetRealCurrentWeather(string location)
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={location}&appid={_apiKey}&units=metric&lang=sv";
            
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(new { error = "Kunde inte hitta angiven plats" });
            }

            var json = await response.Content.ReadAsStringAsync();
            var weatherData = JsonSerializer.Deserialize<OpenWeatherResponse>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return Ok(new
            {
                location = weatherData.Name,
                temperature = weatherData.Main.Temp,
                feelsLike = weatherData.Main.FeelsLike,
                humidity = weatherData.Main.Humidity,
                pressure = weatherData.Main.Pressure,
                windSpeed = weatherData.Wind.Speed,
                main = weatherData.Weather[0].Main,
                description = weatherData.Weather[0].Description,
                icon = weatherData.Weather[0].Icon
            });
        }

        private async Task<IActionResult> GetRealForecast(string location)
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"https://api.openweathermap.org/data/2.5/forecast?q={location}&appid={_apiKey}&units=metric&lang=sv";
            
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(new { error = "Kunde inte hitta angiven plats" });
            }

            var json = await response.Content.ReadAsStringAsync();
            var forecastData = JsonSerializer.Deserialize<OpenWeatherForecastResponse>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Gruppera per dag och ta en prognos per dag
            var dailyForecasts = forecastData.List
                .Where(item => item.DtTxt.Hour >= 12 && item.DtTxt.Hour <= 15) // Ta middagsprognoser
                .Take(5)
                .Select(item => new
                {
                    date = item.DtTxt.Date,
                    temperature = item.Main.Temp,
                    minTemperature = item.Main.TempMin,
                    maxTemperature = item.Main.TempMax,
                    main = item.Weather[0].Main,
                    description = item.Weather[0].Description,
                    icon = item.Weather[0].Icon
                })
                .ToList();

            return Ok(dailyForecasts);
        }

        private IActionResult GetMockCurrentWeather(string location)
        {
            var random = new Random();
            var temp = random.Next(-5, 25);
            var conditions = new[] { "Clear", "Clouds", "Rain", "Snow" };
            var descriptions = new[] { "Klart", "Molnigt", "Regn", "Snö" };
            var conditionIndex = random.Next(conditions.Length);

            return Ok(new
            {
                location = location,
                temperature = temp,
                feelsLike = temp + random.Next(-3, 3),
                humidity = random.Next(30, 90),
                pressure = random.Next(990, 1030),
                windSpeed = random.Next(0, 15),
                main = conditions[conditionIndex],
                description = descriptions[conditionIndex],
                icon = "01d"
            });
        }

        private IActionResult GetMockForecast(string location)
        {
            var random = new Random();
            var conditions = new[] { "Clear", "Clouds", "Rain", "Snow" };
            var descriptions = new[] { "Klart", "Molnigt", "Regn", "Snö" };

            var forecast = Enumerable.Range(1, 5).Select(index =>
            {
                var temp = random.Next(-5, 25);
                var conditionIndex = random.Next(conditions.Length);
                
                return new
                {
                    date = DateTime.Now.AddDays(index).Date,
                    temperature = temp,
                    minTemperature = temp - random.Next(0, 8),
                    maxTemperature = temp + random.Next(0, 8),
                    main = conditions[conditionIndex],
                    description = descriptions[conditionIndex],
                    icon = "01d"
                };
            }).ToList();

            return Ok(forecast);
        }
    }

    // DTO klasser för OpenWeatherMap API
    public class OpenWeatherResponse
    {
        public string Name { get; set; }
        public MainData Main { get; set; }
        public WindData Wind { get; set; }
        public List<WeatherData> Weather { get; set; }
    }

    public class OpenWeatherForecastResponse
    {
        public List<ForecastItem> List { get; set; }
    }

    public class ForecastItem
    {
        public DateTime DtTxt { get; set; }
        public MainData Main { get; set; }
        public List<WeatherData> Weather { get; set; }
    }

    public class MainData
    {
        [JsonPropertyName("temp")]
        public double Temp { get; set; }
        
        [JsonPropertyName("feels_like")]
        public double FeelsLike { get; set; }
        
        [JsonPropertyName("temp_min")]
        public double TempMin { get; set; }
        
        [JsonPropertyName("temp_max")]
        public double TempMax { get; set; }
        
        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }
        
        [JsonPropertyName("pressure")]
        public int Pressure { get; set; }
    }

    public class WindData
    {
        [JsonPropertyName("speed")]
        public double Speed { get; set; }
    }

    public class WeatherData
    {
        public string Main { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
    }
}
