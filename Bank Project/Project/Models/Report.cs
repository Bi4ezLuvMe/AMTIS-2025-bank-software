using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class Report
    {
        public string id { get; set; }
        public DateTime fromTime { get; set; }
        public DateTime toTime { get; set; }
        public int totalApprovedCount { get; set; }
        public decimal totalApprovedAmount { get; set; }
        public int totalDeclinedCount { get; set; }
        public decimal totalDeclinedAmount { get; set; }
        public decimal totalEarningsAmount { get; set; }
        public List<ClientReport> clients { get; set; }
    }
}
