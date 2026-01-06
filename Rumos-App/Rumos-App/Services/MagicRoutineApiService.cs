using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rumos_App.DTOs;
using Rumos_App.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rumos_App.Services
{
    internal class MagicRoutineApiService
    {
        private readonly ILogger<MagicRoutineApiService> _logger;
        private static List<Preset>? _presetList;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string? _baseApiUrl;

        public MagicRoutineApiService(HttpClient httpClient, IConfiguration configuration, ILogger<MagicRoutineApiService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _baseApiUrl = _configuration["ApiSettings:BaseApiUrl"];

            // デバッグログ追加
            _logger.LogInformation("===== MagicRoutineApiService Initialization =====");
            _logger.LogInformation("BaseApiUrl: {BaseApiUrl}", _baseApiUrl ?? "NULL");

            if (string.IsNullOrEmpty(_baseApiUrl))
            {
                _logger.LogError("BaseApiUrl is not configured");
                throw new InvalidOperationException("BaseApiUrl must be configured in ApiSettings");
            }

            var apiKey = _configuration["ApiSettings:RumosApiKey"];

            // デバッグログ追加
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("API Key not found in configuration");
            }
            else
            {
                _logger.LogInformation("API Key found (length: {Length})", apiKey.Length);
                _httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);
                _logger.LogInformation("X-API-Key header added to HttpClient");

                // ヘッダーが実際に追加されたか確認
                var hasKey = _httpClient.DefaultRequestHeaders.Contains("X-API-Key");
                _logger.LogInformation("X-API-Key header exists: {HasKey}", hasKey);
            }

            _logger.LogInformation("===== Initialization Complete =====");
        }
        
        public async Task<List<Preset>> GetPresetList()
        {
            if (_presetList == null)
            {
                await GetAllPresetAsync();
            }
            return _presetList ?? new List<Preset>();
        }

        public async Task GetAllPresetAsync()
        {
            try
            {
                var url = $"{_baseApiUrl}/MagicRoutin";
                _logger.LogInformation("===== Fetching Presets =====");
                _logger.LogInformation("URL: {Url}", url);

                // ヘッダーをログに出力
                var headers = _httpClient.DefaultRequestHeaders;
                foreach (var header in headers)
                {
                    _logger.LogInformation("Header: {Key} = {Value}",
                        header.Key,
                        string.Join(", ", header.Value));
                }

                var result = await _httpClient.GetFromJsonAsync<List<Preset>>(url);
                _presetList = result ?? new List<Preset>();

                _logger.LogInformation("Successfully fetched {Count} preset(s)", _presetList.Count);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while fetching presets. Status Code: {StatusCode}",
                    ex.StatusCode);
                _presetList = new List<Preset>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching presets");
                _presetList = new List<Preset>();
            }
        }

        public async Task<bool> DeletePresetAsync(int id)
        {
            try
            {
                var url = $"{_baseApiUrl}/magicroutin/{id}";
                var response = await _httpClient.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Preset {PresetId} deleted successfully", id);
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Delete failed for preset {PresetId}: {Error}", id, error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting preset {PresetId}", id);
                return false;
            }
        }

        public async Task<Preset?> PostRoutine(PresetCreateDto data)
        {
            if (data?.File == null || string.IsNullOrEmpty(data.Name))
            {
                _logger.LogWarning("PostRoutine called with invalid data (File or Name is null)");
                return null;
            }

            try
            {
                var content = new MultipartFormDataContent();
                var stream = data.File.OpenReadStream(maxAllowedSize: 10 * 1080 * 1920);
                var fileContent = new StreamContent(stream);

                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(data.File.ContentType);
                content.Add(new StringContent(data.Name), "Name");
                content.Add(fileContent, "File", data.File.Name);

                var url = $"{_baseApiUrl}/MagicRoutin";
                var response = await _httpClient.PostAsync(url, content);

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    var preset = await response.Content.ReadFromJsonAsync<Preset>();
                    _logger.LogInformation("Routine created successfully: {PresetData}", JsonSerializer.Serialize(preset));
                    return preset;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Routine creation failed with status {StatusCode}: {Error}",
                        response.StatusCode, error);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating routine for preset: {PresetName}", data.Name);
                return null;
            }
        }

        public async Task<bool> PostRoutineSetting(List<PlatformWithDevicesDto>? withPlatformList, int presetId)
        {
            if (withPlatformList == null || withPlatformList.Count == 0)
            {
                _logger.LogWarning("PostRoutineSetting called with null or empty platform list");
                return false;
            }

            try
            {
                List<DeviceDto> deviceList = new();

                foreach (PlatformWithDevicesDto platform in withPlatformList)
                {
                    foreach (DeviceDto device in platform.Devices)
                    {
                        // デバイスが有効な設定を持っているかチェック
                        if (device == null ||
                            device.Brightness == 0 ||
                            (device.R == 0 && device.G == 0 && device.B == 0))
                        {
                            continue;
                        }

                        deviceList.Add(device);
                    }
                }

                if (deviceList.Count == 0)
                {
                    _logger.LogWarning("No valid devices found for preset {PresetId}", presetId);
                    return false;
                }

                var url = $"{_baseApiUrl}/MagicRoutin/routine/{presetId}";
                var response = await _httpClient.PostAsJsonAsync(url, deviceList);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Routine settings posted successfully for preset {PresetId} with {Count} device(s): {Devices}",
                        presetId, deviceList.Count, JsonSerializer.Serialize(deviceList));
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to post routine settings for preset {PresetId}: {Error}",
                        presetId, error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error posting routine settings for preset {PresetId}", presetId);
                return false;
            }
        }

        public async Task<bool> ExeMagicRoutine(int id, List<PlatformWithDevicesDto>? devices)
        {
            if (devices == null || devices.Count == 0)
            {
                _logger.LogWarning("ExeMagicRoutine called with null or empty devices list");
                return false;
            }

            try
            {
                var url = $"{_baseApiUrl}/Lumina/magicroutin/execution/{id}";
                var response = await _httpClient.PostAsJsonAsync<List<Preset_device_map>>(url, null);

                if (response.IsSuccessStatusCode)
                {
                    var exeValue = await response.Content.ReadFromJsonAsync<List<Preset_device_map>>();
                    List<Preset_device_map> data = exeValue ?? new List<Preset_device_map>();

                    int updatedDeviceCount = 0;
                    foreach (PlatformWithDevicesDto platform in devices)
                    {
                        foreach (DeviceDto device in platform.Devices)
                        {
                            var map = data.FirstOrDefault(d => d.Device_id == device.Id);
                            if (map != null)
                            {
                                device.R = map.R;
                                device.G = map.G;
                                device.B = map.B;
                                device.Brightness = map.Brightness;
                                updatedDeviceCount++;
                            }
                            else
                            {
                                device.R = 0;
                                device.G = 0;
                                device.B = 0;
                                device.Brightness = 0;
                            }
                        }
                    }

                    _logger.LogInformation("Magic routine {RoutineId} executed successfully, updated {Count} device(s)",
                        id, updatedDeviceCount);
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Magic routine execution failed for {RoutineId}: {Error}", id, error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing magic routine {RoutineId}", id);
                return false;
            }
        }

        public async Task<bool> ExeAll(LedColor color, List<PlatformWithDevicesDto>? devices)
        {
            if (devices == null || devices.Count == 0)
            {
                _logger.LogWarning("ExeAll called with null or empty devices list");
                return false;
            }

            if (color == null)
            {
                _logger.LogWarning("ExeAll called with null color");
                return false;
            }

            try
            {
                var url = $"{_baseApiUrl}/Lumina/all";
                var response = await _httpClient.PostAsJsonAsync(url, color);

                if (response.IsSuccessStatusCode)
                {
                    int updatedDeviceCount = 0;
                    foreach (PlatformWithDevicesDto platform in devices)
                    {
                        foreach (DeviceDto device in platform.Devices)
                        {
                            device.R = color.R;
                            device.G = color.G;
                            device.B = color.B;
                            device.Brightness = color.Brightness;
                            updatedDeviceCount++;
                        }
                    }

                    _logger.LogInformation("All devices set to color (R:{R}, G:{G}, B:{B}, Brightness:{Brightness}), updated {Count} device(s)",
                        color.R, color.G, color.B, color.Brightness, updatedDeviceCount);
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to set all devices: {Error}", error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting all devices to color");
                return false;
            }
        }
    }
}