using Rumos_App.Entities;
using System.Net.Http.Json;
using Rumos_App.DTOs;
using Microsoft.Extensions.Configuration;

namespace Rumos_App.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string? baseApiUrl;
        //デバイス一覧をキャッシュしておくための変数
        private static List<PlatformWithDevicesDto> _devicesData;
        
        public ApiService(HttpClient httpClient,IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            baseApiUrl = _configuration["ApiSettings:BaseApiUrl"];
        }

        public async Task<List<PlatformWithDevicesDto>> GetDevicesData()
        {
            if(_devicesData == null)
            {
                await GetAllDevicesWithPlatformAsync();
            }
            return _devicesData ?? new List<PlatformWithDevicesDto>(); 
        }

        public async Task GetAllDevicesWithPlatformAsync()
        {
         
            try
            {
                _httpClient.Timeout = TimeSpan.FromSeconds(5);
                var result = await _httpClient.GetFromJsonAsync<List<PlatformWithDevicesDto>>(baseApiUrl + "/Lumina/deviceswithplatform");
                _devicesData = result ?? new List<PlatformWithDevicesDto>();
            }
            catch (Exception)
            {
                _devicesData = new List<PlatformWithDevicesDto>();
            }

        }

        //デバイスの登録処理:POST
        public async Task<Entities.Device> PostDevice(CreateDeviceDto deviceData)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(baseApiUrl + "/Device/register", deviceData);

                if(response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return await response.Content.ReadFromJsonAsync<Entities.Device>() ?? new Entities.Device();
                }
                else
                {
                    return new Entities.Device();
                }

            }
            catch
            {
                return new Entities.Device();
            }
        }

        //デバイスの削除処理:Delete
        public async Task<bool> DeleteDeviceAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(baseApiUrl + $"/Device/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    // ログ出したければここ
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Delete failed: {error}");
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Entities.Platform>> GetPlatforms()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Entities.Platform>>(baseApiUrl + "/Platform") ?? new List<Entities.Platform>();
            }
            catch
            {
                return new List<Entities.Platform>();
            }
        }


        public async Task<List<Entities.Room>> GetRooms()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Entities.Room>>(baseApiUrl + "/room") ?? new List<Entities.Room>();
            }
            catch
            {
                return new List<Entities.Room>();
            }
        }

       
        public async Task TurnOnTheLightById(DeviceDto? data)
        {
            var dto = new DevicePostDto
            {
                R = data.R,
                G = data.G,
                B = data.B,
                Brightness = data.Brightness,
                Mode = "normal"
            };

            var result = await _httpClient.PostAsJsonAsync(
                $"{baseApiUrl}/Lumina/{data.Id}",
                dto
            );

        }
    }
}
