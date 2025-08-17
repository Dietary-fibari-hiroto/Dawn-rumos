using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Web.Http.Diagnostics;
using System.Text.Json;

namespace rumos.Api
{
    public class RumosApi
    {
        private readonly HttpClient _httpClient;

        public RumosApi(){
            _httpClient = new HttpClient();
        }

        public async Task<string> PostPowerSupply(string url)
        {
            Console.WriteLine($"サプライ処理実行:{url}");
            HttpResponseMessage response = await _httpClient.PostAsync(url,null);
            response.EnsureSuccessStatusCode();
            //return await response.Content.ReadAsStringAsync();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> PostESP(string url)
        {
            var color = new LedColor { R = 47, G = 0, B = 94, Brightness=50 };
            string json = JsonSerializer.Serialize(color);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();

        }
    }
    public class LedColor
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public int Brightness { get; set; }
    }

}
