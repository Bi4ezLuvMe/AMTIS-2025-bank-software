using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Project.Services.PostRequests
{
    public class PostData
    {
        public PostData(string competitorId, string sessionType, string gitSha)
        {
            this.competitorId = competitorId;
            this.sessionType = sessionType;
            this.gitSha = gitSha;
        }

        public string competitorId { get; set; }
        public string sessionType { get; set; }
        public string gitSha { get; set; }
    }
}
