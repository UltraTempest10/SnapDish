using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Service
{
    public class AiService
    {
        private readonly string _apiKey;
        private readonly string _secretKey;
        private readonly HttpClient _httpClient;

        public AiService()
        {
            _apiKey = "Xk7i9jLOZiDmlYINUWJjM5PK";
            _secretKey = "G01Z0zrugQixgJAuHKMo6bQqWFxmuAZM";
            _httpClient = new HttpClient();
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var url = "https://aip.baidubce.com/oauth/2.0/token";
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", _apiKey),
                new KeyValuePair<string, string>("client_secret", _secretKey)
            });

            var response = await _httpClient.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseString);
            return json["access_token"].ToString();
        }

        public async Task<string> GetAnswerAsync(string question)
        {
            var accessToken = await GetAccessTokenAsync();
            var url = $"https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/completions?access_token={accessToken}";

            var payload = new
            {
                temperature = 0.95,
                top_p = 0.8,
                penalty_score = 1,
                disable_search = false,
                enable_citation = false,
                response_format = "text",
                messages = new[]
                {
                new { role = "user", content = question }
            }
            };

            var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseString);

            Console.WriteLine(json);

            return json["result"].ToString();
        }
    }
}