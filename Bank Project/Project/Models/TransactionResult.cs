using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class TransactionResult
    {
        public TransactionResult(string transactionId,string status)
        {
            this.TransactionId = transactionId;
            this.Status = status;   
        }
        public string TransactionId { get; set; }
        public string Status { get; set; }
    }
}
