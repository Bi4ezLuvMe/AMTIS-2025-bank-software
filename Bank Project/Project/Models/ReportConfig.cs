using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Project.Models
{
    public class ReportConfig
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("from_timestamp")]
        public DateTime FromTimestamp { get; set; }

        [JsonPropertyName("to_timestamp")]
        public DateTime ToTimestamp { get; set; }
        [JsonPropertyName("clientIds")]
        public List<string>? ClientIds { get; set; }
    }
}
