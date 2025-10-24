using System.Text.Json;
using TravelDashboard_SOA_CA1.Models;

namespace TravelDashboard_SOA_CA1.Service;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey = "7c5db204b55025a1e50f7a2f492c4c19"; //OpenWeather Api Key

    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient; //Client used to initiate HTTP requests
    }

    public async Task<Weather?> GetWeatherAsync(string city, string? countryCode = null)
    {
        if (string.IsNullOrWhiteSpace(city)) // check the city name is valid
            return null;
        //Construct query parameters
        string query = string.IsNullOrWhiteSpace(countryCode) ? city : $"{city},{countryCode}";
        //Api Link
        string url = $"https://api.openweathermap.org/data/2.5/weather?q={query}&appid={_apiKey}&units=metric";
        try
        {
            var response = await _httpClient.GetFromJsonAsync<JsonElement>(url); //Call the API and get a JSON response
            if (response.ValueKind == JsonValueKind.Undefined)
                return null;

            return new Weather // Parse the JSON response and map it to a Weather object
            {
                CityName = response.GetProperty("name").GetString(),
                Temperature = response.GetProperty("main").GetProperty("temp").GetDouble(),
                Description = response.GetProperty("weather")[0].GetProperty("description").GetString(),
                WindSpeed = response.GetProperty("wind").GetProperty("speed").GetDouble(),
                Humidity = response.GetProperty("main").GetProperty("humidity").GetInt32(),
                Icon = response.GetProperty("weather")[0].GetProperty("icon").GetString(),
                Sunrise = DateTimeOffset
                    .FromUnixTimeSeconds(response.GetProperty("sys").GetProperty("sunrise").GetInt64()).DateTime,
                Sunset = DateTimeOffset
                    .FromUnixTimeSeconds(response.GetProperty("sys").GetProperty("sunset").GetInt64()).DateTime,
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching weather: {ex.Message}");
            return null;
        }
    }

    //Get the future weather forecast for a specified city
    public async Task<List<Weather>> GetForecastWeatherAsync(string city, string? countryCode = null)
    {
        // Create a list to store weather data
        List<Weather> forecastList = new List<Weather>();

        // Check if the city name is empty
        if (string.IsNullOrWhiteSpace(city))
            return forecastList;
        // Combining query strings
        string query = string.IsNullOrWhiteSpace(countryCode) ? city : $"{city},{countryCode}";

        // API link
        string url = $"https://api.openweathermap.org/data/2.5/forecast?q={query}&appid={_apiKey}&units=metric";
        try
        {
            //Get data from APi
            var response = await _httpClient.GetFromJsonAsync<JsonElement>(url);
            // If no valid data is returned
            if (response.ValueKind == JsonValueKind.Undefined)
                return forecastList;
            // Get City's name
            string? cityName = response.GetProperty("city").GetProperty("name").GetString();
            // Get weather list
            var list = response.GetProperty("list").EnumerateArray();
            // Use dictionary to save data everyday
            Dictionary<DateTime, List<Weather>> dailyData = new Dictionary<DateTime, List<Weather>>();
            // Traverse the weather data for each time point returned by the API
            foreach (var item in list)
            {
                DateTime date = DateTimeOffset.FromUnixTimeSeconds(item.GetProperty("dt").GetInt64()).DateTime.Date;
                double tempMin = item.GetProperty("main").GetProperty("temp_min").GetDouble();
                double tempMax = item.GetProperty("main").GetProperty("temp_max").GetDouble();
                string description = item.GetProperty("weather")[0].GetProperty("description").GetString();
                string icon = item.GetProperty("weather")[0].GetProperty("icon").GetString();

                Weather w = new Weather
                {
                    Date = date,
                    TempMin = tempMin,
                    TempMax = tempMax,
                    Description = description,
                    Icon = icon,
                    CityName = cityName
                };
                // Group weather by date
                if (!dailyData.ContainsKey(date))
                {
                    dailyData[date] = new List<Weather>();
                }

                dailyData[date].Add(w);
            }
            // Handling daily averages or extreme values
            foreach (var day in dailyData)
            {
                var date = day.Key;
                var weatherItems = day.Value;
                // Calculate the minimum and maximum temperatures for the day
                double minTemp = weatherItems.Min(w => w.TempMin);
                double maxTemp = weatherItems.Max(w => w.TempMax);
                // Take the middle weather description
                int middleIndex = weatherItems.Count / 2;
                //Get the description and icon of the middle weather
                string desc = weatherItems[middleIndex].Description;
                string icon = weatherItems[middleIndex].Icon;
                Weather dailyWeather = new Weather
                {
                    Date = date,
                    CityName = cityName,
                    TempMin = minTemp,
                    TempMax = maxTemp,
                    Description = desc,
                    Icon = icon
                };
                forecastList.Add(dailyWeather);
            }
            // Sort by date
            forecastList = forecastList.OrderBy(f => f.Date).ToList();
            return forecastList;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching forecast weather: {ex.Message}");
            return forecastList;
        }
    }
}