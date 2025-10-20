using System.Net.Http.Json;
using TravelDashboard_SOA_CA1.Models;

namespace TravelDashboard_SOA_CA1.Service
{
    public class CountryService
    {
        private readonly HttpClient _http;

        public CountryService(HttpClient httpClient)
        {
            _http = httpClient;
        }
        // Get all countries from API For home page!
        public async Task<List<Country>> GetAllCountriesAsync()
        {
            // Get only necessary fields to reduce the amount of data
            // string url = "https://restcountries.com/v3.1/all?fields=name,flags,population,region,subregion,capital,languages";
            string url = "https://restcountries.com/v3.1/all?fields=name,flags";
            var countries = new List<Country>();
            try
            {
                var result = await _http.GetFromJsonAsync<List<Country.CountryApiModel>>(url);
                if (result == null) return countries;
                foreach (var c in result)
                {
                    countries.Add(new Country
                    {
                        Name = c.name.common,
                        Flag = c.flags.png,
                        // Population = c.population,
                        // Region = c.region,
                        // Subregion = c.subregion,
                        // Capital = c.capital != null && c.capital.Count > 0 ? c.capital[0] : "N/A",
                        // Languages = c.languages != null ? string.Join(", ", c.languages.Values) : "N/A"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching all countries: {ex.Message}");
            }
            return countries.OrderBy(c => c.Name).ToList();
        }
        // Get a single country details for the details page (ShowCountry.razor)
        public async Task<Country?> GetCountryByNameAsync(string name)
        {
            // Get multiple data for a country
            string url = $"https://restcountries.com/v3.1/name/{name}?fullText=true&fields=name,flags,population,region,subregion,capital,languages";

            try
            {
                var result = await _http.GetFromJsonAsync<List<Country.CountryApiModel>>(url);
                
                // If the API returns data, we only take the first country
                var c = result?.FirstOrDefault();

                if (c == null) return null;

                return new Country
                {
                    Name = c.name.common,
                    Flag = c.flags.png,
                    Population = c.population,
                    Region = c.region,
                    Subregion = c.subregion,
                    Capital = c.capital != null && c.capital.Count > 0 ? c.capital[0] : "N/A",
                    Languages = c.languages != null ? string.Join(", ", c.languages.Values) : "N/A"
                };
            }
            catch (Exception ex)
            {
                //GetFromJsonAsync will throw an exception, so we can just return null
                Console.WriteLine($"Error fetching country '{name}': {ex.Message}");
                return null;
            }
        }
    }
}