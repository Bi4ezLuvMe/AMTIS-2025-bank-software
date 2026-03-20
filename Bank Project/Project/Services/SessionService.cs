using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BankingCompetition.Services
{
    public class SessionService : ISessionService
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl;
        private readonly string _competitorId;
        private readonly string _gitSha;
        private readonly string _sessionType;

        public string SessionId { get; private set; }

        public SessionService(HttpClient client, string baseUrl, string competitorId, string gitSha, string sessionType)
        {
            _client = client;
            _baseUrl = baseUrl;
            _competitorId = competitorId;
            _gitSha = gitSha;
            _sessionType = sessionType;
        }

        public async Task<bool> InitializeSessionAsync()
        {
            var payload = new
            {
                competitorId = _competitorId,
                sessionType = _sessionType,
                gitSha = _gitSha
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(_baseUrl + "sessions", content);

            if (!response.IsSuccessStatusCode)
                return false;

            var responseBody = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;

            SessionId = root.GetProperty("sessionId").GetString();

            return true;
        }
    }
}
