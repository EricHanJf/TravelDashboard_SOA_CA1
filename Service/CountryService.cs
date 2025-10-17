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

        public async Task<List<Country>> GetAllCountriesAsync()
        {
            string url = "https://restcountries.com/v3.1/all?fields=name,flags,population,region,subregion,capital,languages";
            var countries = new List<Country>();

            try
            {
                var result = await _http.GetFromJsonAsync<List<Country.CountryApiModel>>(url);
                if (result == null) return countries;

                foreach (var c in result)
                {
                    string languageList = "N/A";
                    if (c.languages != null && c.languages.Count > 0)
                    {
                        languageList = string.Join(", ", c.languages.Values);
                    }
                    countries.Add(new Country
                    {
                        Name = c.name.common,
                        Flag = c.flags.png,
                        Population = c.population,
                        Region = c.region,
                        Subregion = c.subregion,
                        Capital = c.capital != null && c.capital.Count > 0 ? c.capital[0] : "N/A",
                        Languages = languageList
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching countries: {ex.Message}");
            }

            return countries.OrderBy(c => c.Name).ToList();
        }
    }
}