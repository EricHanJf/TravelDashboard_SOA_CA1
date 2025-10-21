namespace TravelDashboard_SOA_CA1.Models;

public class Country
{
    // Properties for display
    public string Name { get; set; }
    public string Flag { get; set; }
    public long Population { get; set; }
    public string Region { get; set; }
    public string Subregion { get; set; }
    public string Capital { get; set; }
    public string Languages { get; set; }
    public string? GoogleMapURL { get; set; }

    // Data structure that conforms to API JSON
    public class CountryApiModel
    {
        public NameInfo name { get; set; }
        public FlagsInfo flags { get; set; }
        public long population { get; set; }
        public string region { get; set; }
        public string subregion { get; set; }
        public List<string>? capital { get; set; }
        public Dictionary<string, string>? languages { get; set; }
        public MapsInfo maps { get; set; }

        public class MapsInfo
        {
            public string googleMaps { get; set; }
        }

        public class NameInfo
        {
            public string common { get; set; }
        }

        public class FlagsInfo
        {
            public string png { get; set; }
        }
    }
}