using ChatPanel.Models;
using System.Text.Json;

namespace ChatPanel.Services
{
    public class FirebaseService
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;
        private readonly string _secret;

        public FirebaseService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _baseUrl = config["Firebase:DatabaseUrl"].TrimEnd('/');
            _secret = config["Firebase:DatabaseSecret"];
        }

        private string Url(string path) =>
            $"{_baseUrl}/{path.TrimStart('/')}.json?auth={_secret}";

        public async Task<List<Message>> GetMessagesAsync(string roomId)
        {
            var response = await _http.GetAsync(Url($"rooms/{roomId}/messages"));
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json) || json == "null")
                return new List<Message>();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var dict = JsonSerializer.Deserialize<Dictionary<string, Message>>(json, options);

            return dict.Select(kv =>
            {
                kv.Value.MessageId = kv.Key;
                return kv.Value;
            }).OrderByDescending(m => m.CreatedAt).ToList();
        }

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
