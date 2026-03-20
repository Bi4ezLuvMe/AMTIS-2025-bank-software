using BankingCompetition.Models;
using Project.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankingCompetition.Services
{
    public interface ITransactionService
    {
        Task<TransactionBatch> GetTransactionBatchAsync(string sessionId, string competitorId);
        List<TransactionResult> ProcessTransactions(List<Transaction> transactions);
        Task<bool> SendBatchResultsAsync(string sessionId, string competitorId, string batchId, List<TransactionResult> results, string resultsHash);
        Task<List<Transaction>> LoadTransactionsAsync(HttpClient client, string sessionId, string competitorId);
    }
}
