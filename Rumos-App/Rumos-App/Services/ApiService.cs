using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rumos_App.DTOs;
using Rumos_App.Entities;
using System.Net.Http.Json;

namespace Rumos_App.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string? _baseApiUrl;
        private static List<PlatformWithDevicesDto>? _devicesData;
        private readonly ILogger<ApiService> _logger;

        public ApiService(HttpClient httpClient, IConfiguration configuration, ILogger<ApiService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var apiKey = _configuration["ApiSettings:RumosApiKey"];
            if (!string.IsNullOrEmpty(apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);
            }


            _baseApiUrl = _configuration["ApiSettings:BaseApiUrl"];

            if (string.IsNullOrEmpty(_baseApiUrl))
            {
                _logger.LogError("BaseApiUrl is not configured");
                throw new InvalidOperationException("BaseApiUrl must be configured in ApiSettings");
            }

            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<List<PlatformWithDevicesDto>> GetDevicesData()
        {
            if (_devicesData == null)
            {
                await GetAllDevicesWithPlatformAsync();
            }
            return _devicesData ?? new List<PlatformWithDevicesDto>();
        }

        public async Task GetAllDevicesWithPlatformAsync()
        {
            try
            {
                var url = $"{_baseApiUrl}/Lumina/deviceswithplatform";
                _logger.LogInformation("Fetching devices from: {Url}", url);

                var result = await _httpClient.GetFromJsonAsync<List<PlatformWithDevicesDto>>(url);
                _devicesData = result ?? new List<PlatformWithDevicesDto>();

                _logger.LogInformation("Successfully fetched {Count} platform(s) with devices", _devicesData.Count);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while fetching devices");
                _devicesData = new List<PlatformWithDevicesDto>();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Request timeout while fetching devices");
                _devicesData = new List<PlatformWithDevicesDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching devices");
                _devicesData = new List<PlatformWithDevicesDto>();
            }
        }

        public async Task<Entities.Device?> PostDevice(CreateDeviceDto deviceData)
        {
            try
            {
                var url = $"{_baseApiUrl}/Device/register";
                var response = await _httpClient.PostAsJsonAsync(url, deviceData);

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    var device = await response.Content.ReadFromJsonAsync<Entities.Device>();
                    _logger.LogInformation("Device created successfully with ID: {DeviceId}", device?.Id);
                    return device;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Device creation failed with status {StatusCode}: {Error}",
                        response.StatusCode, errorContent);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating device");
                return null;
            }
        }

        public async Task<bool> DeleteDeviceAsync(int id)
        {
            try
            {
                var url = $"{_baseApiUrl}/Device/{id}";
                var response = await _httpClient.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Device {DeviceId} deleted successfully", id);
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Delete failed for device {DeviceId}: {Error}", id, error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting device {DeviceId}", id);
                return false;
            }
        }

        public async Task<List<Entities.Platform>> GetPlatforms()
        {
            try
            {
                var url = $"{_baseApiUrl}/Platform";
                return await _httpClient.GetFromJsonAsync<List<Entities.Platform>>(url)
                    ?? new List<Entities.Platform>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching platforms");
                return new List<Entities.Platform>();
            }
        }

        public async Task<List<Entities.Room>> GetRooms()
        {
            try
            {
                var url = $"{_baseApiUrl}/room";
                return await _httpClient.GetFromJsonAsync<List<Entities.Room>>(url)
                    ?? new List<Entities.Room>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rooms");
                return new List<Entities.Room>();
            }
        }

        public async Task<bool> TurnOnTheLightById(DeviceDto? data)
        {
            if (data == null)
            {
                _logger.LogWarning("TurnOnTheLightById called with null data");
                return false;
            }

            try
            {
                var dto = new DevicePostDto
                {
                    R = data.R,
                    G = data.G,
                    B = data.B,
                    Brightness = data.Brightness,
                    Mode = "normal"
                };

                var url = $"{_baseApiUrl}/Lumina/{data.Id}";
                var response = await _httpClient.PostAsJsonAsync(url, dto);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Light {DeviceId} turned on successfully", data.Id);
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to turn on light {DeviceId}: {Error}", data.Id, error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error turning on light {DeviceId}", data?.Id);
                return false;
            }
        }
    }
}