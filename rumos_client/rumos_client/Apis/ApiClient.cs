using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

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
    }
}
