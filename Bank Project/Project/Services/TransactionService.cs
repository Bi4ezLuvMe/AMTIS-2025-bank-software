using BankingCompetition.Models;
using BankingCompetition.Utils;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BankingCompetition.Services
{
    public class TransactionService
    {
        private readonly HttpClient _client;

        private Dictionary<string, List<Transaction>> _transactionsByCard = new();
        private Dictionary<string, List<Transaction>> _transactionsByUser = new();


        private decimal StandardCardDailyLimit;
        private decimal StandardCardWeeklyLimit;
        private decimal PremiumCardDailyLimit;
        private decimal PremiumCardWeeklyLimit;
        private decimal UserDailyLimit;
        private decimal UserWeeklyLimit;
        private int MaxTransactionsPer10Seconds;
        private decimal BankFeePercent;

        public TransactionService(
            HttpClient client
            ,decimal StandardCardDailyLimit
            ,decimal StandardCardWeeklyLimit
            ,decimal PremiumCardDailyLimit
            ,decimal PremiumCardWeeklyLimit
            ,decimal UserDailyLimit
            ,decimal UserWeeklyLimit
            ,int MaxTransactionsPer10Seconds
            ,decimal BankFeePercent)
        {
            _client = client;
            this.StandardCardDailyLimit = StandardCardDailyLimit;
            this.StandardCardWeeklyLimit = StandardCardWeeklyLimit;
            this.PremiumCardDailyLimit = PremiumCardDailyLimit;
            this.PremiumCardWeeklyLimit = PremiumCardWeeklyLimit;
            this.UserDailyLimit = UserDailyLimit;
            this.UserWeeklyLimit = UserWeeklyLimit;
            this.MaxTransactionsPer10Seconds = MaxTransactionsPer10Seconds;
            this.BankFeePercent = BankFeePercent;
        }

        public List<TransactionResult> ProcessTransactions(List<Transaction> transactions)
        {
            var results = new List<TransactionResult>();

            foreach (Transaction currentTransaction in transactions)
            {
                if(currentTransaction.type == "authorization")
                {
                    if (_transactionsByCard.ContainsKey(currentTransaction.card_id) == false)
                    {
                        _transactionsByCard.Add(currentTransaction.card_id, new List<Transaction>());
                    }

                }
            }

            return results;
        }

        public async Task<bool> SendBatchResultsAsync(string sessionId, string competitorId, string batchId, List<TransactionResult> results, string resultsHash)
        {
            var json = JsonSerializer.Serialize(results);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Patch, $"transaction-batches/{batchId}");
            request.Headers.Add("Session-Id", sessionId);
            request.Headers.Add("Competitor-Id", competitorId);
            request.Headers.Add("Results-Hash", resultsHash);
            request.Content = content;

            var response = await _client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
