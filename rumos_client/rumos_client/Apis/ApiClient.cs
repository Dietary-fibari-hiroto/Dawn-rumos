using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using rumos_client.Models;

namespace rumos_client.Apis
{

    public class ApiClient
    {
        private readonly HttpClient _http = new HttpClient();
        const string baseUrl = "https://localhost:7032/api";

        //全取得用
        public async Task<T?> GetAsync<T>(string url)
        {
            try
            {
                var response = await _http.GetStringAsync(baseUrl+url);
                return JsonSerializer.Deserialize<T>(response);
            }
            catch (HttpRequestException)
            {
                //サーバー側のエラーかな
                return default;
            }
            catch (JsonException)
            {

                return default;
            }
        }

        //IDを使って一個だけgetする用
        public async Task<T?> GetByIdAsync<T>(string endPoint,int id)
        {
            try
            {
                var responce = await _http.GetStringAsync(baseUrl + endPoint +$"/{id}");
                return JsonSerializer.Deserialize<T>(responce);
            }
            catch (HttpRequestException) { return default; }
            catch (JsonException) { return default; }

        }

        //IDを使ってTpの情報を受け取る
        public async Task<RGetStatusReply> GetTpState(int id)
        {
            try
            {
                var responce = await _http.GetStringAsync(baseUrl + "/device/tp/state/" + id);
                return JsonSerializer.Deserialize<RGetStatusReply>(responce);
            }
            catch (HttpRequestException) { return new RGetStatusReply(isConnect: false, isOn: false); }
            catch (JsonException) { return new RGetStatusReply(isConnect: false, isOn: false); }
        }

        public async Task<RSetPowerReply> PostPowerSupply(int id)
        {
            try
            {
                var response = await _http.PostAsync(baseUrl + "/device/tp/supply/" + id,null);
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<RSetPowerReply>(json);
            }catch (HttpRequestException) { return new RSetPowerReply(); }
            catch (JsonException) { return new RSetPowerReply(); }

        }
    }
}
