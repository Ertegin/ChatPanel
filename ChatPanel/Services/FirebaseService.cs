using ChatPanel.Models;
using System.Text.Json;

namespace ChatPanel.Services
{
    public class FirebaseService
    {
        private readonly HttpClient _http; //HTTP istekleri
        private readonly string _baseUrl;
        private readonly string _secret;

        public FirebaseService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _baseUrl = config["Firebase:DatabaseUrl"].TrimEnd('/'); // / siler
            _secret = config["Firebase:DatabaseSecret"];
        }
      
        private string Url(string path) =>
            $"{_baseUrl}/{path.TrimStart('/')}.json?auth={_secret}";

       /* public async Task<List<Message>> GetMessagesAsync(string roomId)
        {
            var response = await _http.GetAsync(Url($"rooms/{roomId}/messages"));
            response.EnsureSuccessStatusCode();  // 200 olmadığı dırımlarda ex

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json) || json == "null")
                return new List<Message>();
//            catch (HttpRequestException ex)
//{
//                Console.WriteLine($"İstek başarısız: {ex.StatusCode} - {ex.Message}");
//                return null;
//            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true }; //json harf duyarlılığı kapatır
            var dict = JsonSerializer.Deserialize<Dictionary<string, Message>>(json, options);  // json to c#

            return dict.Select(kv =>
            {
                kv.Value.MessageId = kv.Key;
                return kv.Value;
            }).OrderByDescending(m => m.CreatedAt).ToList();
        }*/

        public async Task ApproveMessageAsync(string roomId, string messageId, string approvedBy)
        {
            var payload = new { isApproved = true, approvedBy };
            var response = await _http.PatchAsJsonAsync(
                Url($"rooms/{roomId}/messages/{messageId}"), payload);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteMessageAsync(string roomId, string messageId)
        {
            var response = await _http.DeleteAsync(Url($"rooms/{roomId}/messages/{messageId}"));
            response.EnsureSuccessStatusCode();
        }
    }
}
