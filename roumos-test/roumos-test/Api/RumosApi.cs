using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

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
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
