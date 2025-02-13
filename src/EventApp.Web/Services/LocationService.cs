using System.Text.Json;

namespace EventApp.Web.Services;

public interface ILocationService
{
    Task<LocationResult> GeocodeAddressAsync(string address);
}

public class LocationResult
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string FormattedAddress { get; set; } = string.Empty;
}

public class LocationService : ILocationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LocationService> _logger;

    public LocationService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<LocationService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<LocationResult> GeocodeAddressAsync(string address)
    {
        try
        {
            var apiKey = _configuration["GoogleMaps:ApiKey"];
            var encodedAddress = Uri.EscapeDataString(address);
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={encodedAddress}&key={apiKey}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GoogleGeocodeResponse>(content);

            if (result?.Status != "OK" || result.Results.Count == 0)
            {
                throw new Exception($"Geocoding failed with status: {result?.Status}");
            }

            var location = result.Results[0].Geometry.Location;
            return new LocationResult
            {
                Latitude = location.Lat,
                Longitude = location.Lng,
                FormattedAddress = result.Results[0].FormattedAddress
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error geocoding address: {Address}", address);
            throw;
        }
    }
}

// Google Geocoding API response models
internal class GoogleGeocodeResponse
{
    public string Status { get; set; } = string.Empty;
    public List<GoogleGeocodeResult> Results { get; set; } = new();
}

internal class GoogleGeocodeResult
{
    public string FormattedAddress { get; set; } = string.Empty;
    public GoogleGeometry Geometry { get; set; } = new();
}

internal class GoogleGeometry
{
    public GoogleLocation Location { get; set; } = new();
}

internal class GoogleLocation
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}