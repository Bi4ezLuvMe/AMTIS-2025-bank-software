using Project.Models.SessionConstraints.CardInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.SessionConstraints
{
    public class SpendingLimits
    {
        public int dailyClientLimit { get; set; }
        public int weeklyClientLimit { get; set; }
        public int allowedTransactionsPer10s { get; set; }
        public double interchangeFeePercentage { get; set; }
        public CardLimits cardLimits { get; set; }
    }
}
