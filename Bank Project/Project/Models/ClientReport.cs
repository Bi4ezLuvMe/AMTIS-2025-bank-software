using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class ClientReport
    {
        public string clientId { get; set; }
        public int totalApprovedCount { get; set; }
        public decimal totalApprovedAmount { get; set; }
        public int totalDecliendCount { get; set; }
        public decimal totalDeclinedAmount { get; set; }
        public decimal totalEarningsAmount { get; set; }
    }
}