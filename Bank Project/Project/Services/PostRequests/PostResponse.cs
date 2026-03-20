using Project.Models.SessionConstraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Project.Services.PostRequests
{
    public class PostResponse
    {
        public SessionInfo SessionInfo { get; set; }

        public async Task<SessionInfo> GetSessionConfigAsync()
        {
            using var httpClient = new HttpClient();

            var response = await httpClient.GetAsync("https://competition-engine-production.up.railway.app/init-session");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true 
            };

            var sessionData = JsonSerializer.Deserialize<SessionInfo>(json, options);
            return sessionData;
        }
    }
}
