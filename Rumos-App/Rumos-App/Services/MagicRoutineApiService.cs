using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rumos_App.DTOs;
using Rumos_App.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rumos_App.Services
{
    internal class MagicRoutineApiService
    {
        private readonly ILogger<MagicRoutineApiService> _logger;
        //このクラスで保持しておくデータ
        private static List<Preset> _presetList;
        //コンストラクタ初期化
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string? baseApiUrl;

        public MagicRoutineApiService(HttpClient httpClient, IConfiguration configuration, ILogger<MagicRoutineApiService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            baseApiUrl = _configuration["ApiSettings:BaseApiUrl"];
            _logger = logger;
        }

        //一覧反映
        public async Task<List<Preset>> GetPresetList()
        {
            if (_presetList == null)
            {
                await GetAllPresetAsync();
            }
            return _presetList ?? new List<Preset>();
        }

        //一覧取得
        public async Task GetAllPresetAsync()
        {
            Debug.WriteLine("関数に侵入");
            try
            {
                Debug.WriteLine("TimeSpan後");
                var result = await _httpClient.GetFromJsonAsync<List<Preset>>(baseApiUrl + "/MagicRoutin");
                Debug.WriteLine("API取得失敗してるな？");
                _presetList = result ?? new List<Preset>();
            }
            catch (Exception)
            {
                _presetList = new List<Preset>();
            }
        }
        //削除処理
        public async Task<bool> DeletePresetAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(baseApiUrl + $"/magicroutin/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"削除失敗: {error}");
                    return false;
                }
            }
            catch(Exception e)
            {
                return false;
            }
        }

        //ルーチン登録
        public async Task<Preset> PostRoutine(PresetCreateDto data)
        {
            if(data.File == null || data.Name == null)
            {
                //ファイルがない時の処理あとで
                Console.WriteLine("リターン");
                return null;
            }


            var content = new MultipartFormDataContent();
            var stream = data.File.OpenReadStream(maxAllowedSize: 10 * 1080 * 1920);
            var fileContent = new StreamContent(stream);

            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(data.File.ContentType);
            content.Add(new StringContent(data.Name), "Name");
            content.Add(fileContent, "File",data.File.Name);

            var response = await _httpClient.PostAsync(baseApiUrl + "/MagicRoutin", content);
            if (response.StatusCode == HttpStatusCode.Created)
            {
                var preset = await response.Content.ReadFromJsonAsync<Preset>();
                Debug.WriteLine("Upload result: " + JsonSerializer.Serialize(preset));
                return preset ?? new Preset();
            }
            else
            {
                Debug.WriteLine($"Else result: {response.StatusCode}");
                return new Preset() ;
            }

        }
        
        //発行パターン一覧登録
        public async Task<bool> PostRouitneSetting(List<PlatformWithDevicesDto> withPlatformList,int presetId) 
        {
            if (withPlatformList == null) return false;
            List<DeviceDto> deviceList = new();

            foreach(PlatformWithDevicesDto platform in withPlatformList)
            {
                foreach(DeviceDto device in platform.Devices)
                {
                    if (device == null ||
                        device.Brightness==0 ||
                        (device.R == 0 && device.G == 0 && device.B == 0)) 
                        continue;

                    deviceList.Add(device);
                }
            }
            try
            {
                if (deviceList == null) return false;
                var response = await _httpClient.PostAsJsonAsync(baseApiUrl + $"/MagicRoutin/routine/{presetId}", deviceList);
            }
            catch
            {
                return false;
            }
            Debug.WriteLine(JsonSerializer.Serialize(deviceList));
            return true;
        }

        //一括発光実行
        public async Task<bool> ExeMagicRoutine(int id, List<PlatformWithDevicesDto> devices)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync<List<Preset_device_map>>(baseApiUrl + $"/Lumina/magicroutin/execution/{id}", null);
                if (response.IsSuccessStatusCode)
                {
                    var exeValue = await response.Content.ReadFromJsonAsync<List<Preset_device_map>>();
                    List<Preset_device_map> data = exeValue ?? new List<Preset_device_map>();

                    foreach (PlatformWithDevicesDto platform in devices)
                    {
                        foreach(DeviceDto device in platform.Devices)
                        {
                            var map = data.FirstOrDefault(d => d.Device_id == device.Id);
                            if(map != null)
                            {
                                device.R = map.R;
                                device.G = map.G;
                                device.B = map.B;
                                device.Brightness= map.Brightness;
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
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"実行: {error}");
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExeAll(LedColor color, List<PlatformWithDevicesDto> devices)
        {
            var response = await _httpClient.PostAsJsonAsync(baseApiUrl + "/Lumina/all", color);
            if (response.IsSuccessStatusCode)
            {
                foreach(PlatformWithDevicesDto platform in devices)
                {
                    foreach(DeviceDto device in platform.Devices)
                    {
                        device.R = color.R; device.G = color.G; device.B = color.B;
                        device.Brightness = color.Brightness;

                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
