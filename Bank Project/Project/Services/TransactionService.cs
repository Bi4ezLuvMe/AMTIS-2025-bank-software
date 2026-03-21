using BankingCompetition.Models;
using Project.Models;
using Project.Models.SessionConstraints;
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
        private readonly HttpClient client = new HttpClient();

        private Dictionary<string, List<Transaction>> transactionsByCard = new();
        private Dictionary<string, List<Transaction>> transactionsByUser = new();
        

        private readonly string baseUrl = Project.Common.Constants.baseUrl;
        private readonly string competitorId = Project.Common.Constants.competitorId;
       
        public string SessionId { get;private set; }

        public TransactionService(string sessionId)
        {
            this.SessionId = sessionId;
        }
        public async Task<TransactionBatch> GetTransactionBatchesAsync()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, baseUrl + "transaction-batches");
            request.Headers.Add("Session-Id", this.SessionId);
            request.Headers.Add("Competitor-Id", this.competitorId);

            HttpResponseMessage response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("There was an error getting the transaction batches!" + response.StatusCode);
            }

            string responseContent = await response.Content.ReadAsStringAsync();

            TransactionBatch transactionBatches = JsonSerializer.Deserialize<TransactionBatch>(responseContent);

            return transactionBatches;
        }
        public List<TransactionResult> ProcessTransactions(List<Transaction> transactions)
        {
            var results = new List<TransactionResult>();

            foreach (Transaction currentTransaction in transactions)
            {
                if(currentTransaction.type == "authorization")
                {
                    if (transactionsByCard.ContainsKey(currentTransaction.card_id) == false)
                    {
                        transactionsByCard.Add(currentTransaction.card_id, new List<Transaction>());
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

            var response = await client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
