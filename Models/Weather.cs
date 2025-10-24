namespace TravelDashboard_SOA_CA1.Models;

public class Weather
{
    public string? CityName { get; set; }
    public double Temperature { get; set; }
    public string? Description { get; set; }
    public double WindSpeed { get; set; }
    public int Humidity { get; set; }
    public string? Icon { get; set; }
    public DateTime Sunrise { get; set; }
    public DateTime Sunset { get; set; }
    public DateTime? Date { get; set; } 
    public double TempMin { get; set; } 
    public double TempMax { get; set; } 
}