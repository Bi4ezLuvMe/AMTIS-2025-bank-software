using BankingCompetition.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class TransactionBatch
    {
        public string transactionsBatchId { get; set; }
        public Transaction[]? transactions { get; set; }
    }
}
