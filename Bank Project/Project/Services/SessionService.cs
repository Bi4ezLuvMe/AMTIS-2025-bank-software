using Project.Models.SessionConstraints;
using Project.Services.PostRequests;
using System.Text;
using System.Text.Json;

namespace BankingCompetition.Services
{
    public class SessionService : ISessionService
    {
        private readonly HttpClient client = new HttpClient();
        private readonly string baseUrl = Project.Common.Constants.baseUrl;
        private readonly string competitorId = Project.Common.Constants.competitorId;
        private readonly string gitSha = Project.Common.Constants.gitSha;
        private readonly string sessionType = Project.Common.Constants.sessionType;
        private readonly HashSet<string> allowedSessions = new HashSet<string> {
            "test",
            "sanity",
            "stress",
            "survival"
        };

        public string SessionId { get; private set; }

        public SessionService()
        {
            this.ValidateData(competitorId, gitSha, sessionType, baseUrl);
        }

        public async Task InitializeSessionAsync()
        {
            PostData data = new PostData(this.competitorId, this.sessionType, this.gitSha);

            string json = JsonSerializer.Serialize(data);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(baseUrl + "sessions", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Cound not initialize session!" + response.StatusCode);
            }

            string responseBody = await response.Content.ReadAsStringAsync();

            SessionInfo session = JsonSerializer.Deserialize<SessionInfo>(responseBody);

            this.SessionId = session.sessionId;
        }

        private void ValidateData(string competitorId, string sessionType, string gitSha, string baseUrl)
        {
            if (competitorId == null || competitorId == String.Empty)
            {
                throw new InvalidDataException("Invalid competitorId!");
            }
            if (allowedSessions.Contains(sessionType))
            {
                throw new InvalidDataException("Invalid session type!");
            }
            if (gitSha == null || gitSha == String.Empty)
            {
                throw new InvalidDataException("Invalid gitSha!");
            }
            if (baseUrl == null || baseUrl == String.Empty)
            {
                throw new InvalidDataException("Invalid base url!");
            }
        }
    }
}
