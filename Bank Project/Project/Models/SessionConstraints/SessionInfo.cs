using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.SessionConstraints
{
    public class SessionInfo
    {
        public string sessionId { get; set; }
        public SpendingLimits spendingLimits { get; set; }
    }
}
