using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class ReportConfig
    {
        public string Id { get; set; }
        public DateTime FromTimestamp { get; set; }
        public DateTime ToTimestamp { get; set; }
        public List<string> ClientIds { get; set; }
    }
}
