using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Project.Services.PostRequests
{
    public class PostData
    {
        [JsonPropertyName("competitorId")]
        public string CompetitorId { get; set; }

        [JsonPropertyName("sessionType")]
        public string SessionType { get; set; }

        [JsonPropertyName("gitSha")]
        public string GitSha { get; set; }

        public PostData(string id, string type, string sha)
        {
            CompetitorId = id;
            SessionType = type;
            GitSha = sha;
        }
    }
}
