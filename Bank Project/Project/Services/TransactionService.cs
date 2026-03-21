using BankingCompetition.Models;
using Microsoft.VisualBasic;
using Project.Models;
using Project.Models.SessionConstraints;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.TimeZoneInfo;

namespace BankingCompetition.Services
{
    public class TransactionService
    {
        private readonly HttpClient client = new HttpClient();

        private Dictionary<string, List<TransactionResult>> transactionsByCard = new();
        private Dictionary<string, List<TransactionResult>> transactionsByUser = new();

        private readonly string baseUrl = Project.Common.Constants.baseUrl;
        private readonly string competitorId = Project.Common.Constants.competitorId;
        private string batchId;

        public SessionInfo SessionInfo { get; private set; }

        public TransactionService(SessionInfo sessionInfo)
        {
            this.SessionInfo = sessionInfo;
        }
        public async Task<TransactionBatch> GetTransactionBatchesAsync()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, baseUrl + "transaction-batches");
            request.Headers.Add("Session-Id", this.SessionInfo.sessionId);
            request.Headers.Add("Competitor-Id", this.competitorId);

            HttpResponseMessage response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("There was an error getting the transaction batches!" + response.StatusCode);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return null;
            }

            string responseContent = await response.Content.ReadAsStringAsync();

            TransactionBatch transactionBatches = JsonSerializer.Deserialize<TransactionBatch>(responseContent);

            this.batchId = transactionBatches.transactionsBatchId;

            return transactionBatches;
        }

        public async Task<(List<TransactionResult>,List<Transaction>)> ProcessTransactions(List<Transaction> transactions)
        {
            decimal dailyClientSpendingLimit = SessionInfo.spendingLimits.dailyClientLimit;
            decimal weeklyClientSpendingLimit = SessionInfo.spendingLimits.weeklyClientLimit;
            int transactionsIn10secWindow = SessionInfo.spendingLimits.allowedTransactionsPer10s;
            double bankFee = SessionInfo.spendingLimits.interchangeFeePercentage;
            decimal dailyStandartCardLimit = SessionInfo.spendingLimits.cardLimits.standard.dailyLimit;
            decimal weeklyStandartCardLimit = SessionInfo.spendingLimits.cardLimits.standard.weeklyLimit;
            decimal dailyPremiumCardLimit = SessionInfo.spendingLimits.cardLimits.premium.dailyLimit;
            decimal weeklyPremiumCardLimit = SessionInfo.spendingLimits.cardLimits.premium.weeklyLimit;

            List<TransactionResult> allTransactions = new List<TransactionResult>();
            List<Transaction> allTransactions2 = new List<Transaction>();
            List<Transaction> allApprovedTransactions = new List<Transaction>();

            foreach (Transaction currentTransaction in transactions)
            {
                string status = "approved";

                string cardId = currentTransaction.card_id;
                string cardType = currentTransaction.card_type;
                DateTime transcationDate = currentTransaction.timestamp;
                string clientId = currentTransaction.client_id;

                bool dailyLimits = ControlOfDailyLimitsByCardAndUser(
                    allApprovedTransactions,
                    dailyClientSpendingLimit,
                    dailyStandartCardLimit,
                    dailyPremiumCardLimit,
                    cardId,
                    cardType,
                    transcationDate,
                    clientId);

                bool weeklyLimits = ControlOfWeeklyLimitsByCardAndUser(
                    allApprovedTransactions,
                    weeklyClientSpendingLimit,
                    weeklyStandartCardLimit,
                    weeklyPremiumCardLimit,
                    cardId,
                    cardType,
                    transcationDate,
                    clientId
                    );

                bool tenSecondWindowTransactionLimits = ControlOf10secWindow(
                    allApprovedTransactions,
                    transcationDate,
                    transactionsIn10secWindow,
                    clientId);

                if (!dailyLimits || !weeklyLimits || !tenSecondWindowTransactionLimits)
                {
                    status = "declined";
                }

                if (currentTransaction.type == "authorization")
                {
                    if (transactionsByCard.ContainsKey(currentTransaction.card_id) == false)
                    {
                        transactionsByCard.Add(currentTransaction.card_id,
                            new List<TransactionResult>()
                            { new TransactionResult(currentTransaction.transaction_id,status) });
                    }
                    else
                    {
                        transactionsByCard[currentTransaction.card_id].Add(
                            new TransactionResult(currentTransaction.transaction_id, status));
                    }

                    if (transactionsByUser.ContainsKey(currentTransaction.client_id) == false)
                    {
                        transactionsByUser.Add(currentTransaction.client_id,
                           new List<TransactionResult>()
                           { new TransactionResult(currentTransaction.transaction_id,status) });
                    }
                    else
                    {
                        transactionsByUser[currentTransaction.client_id].Add(
                              new TransactionResult(currentTransaction.transaction_id, status));
                    }
                }
                else if (currentTransaction.type == "refund")
                {
                    Transaction? originalAuth = allApprovedTransactions
        .OrderByDescending(x => x.timestamp) //взимаме първата
        .FirstOrDefault(x =>
            x.type == "authorization" &&
            x.card_id == currentTransaction.card_id &&
            x.client_id == currentTransaction.client_id &&
            x.amount >= currentTransaction.amount &&
            x.timestamp < currentTransaction.timestamp &&
            (currentTransaction.timestamp - x.timestamp).TotalDays <= 7
        );

                    if (originalAuth == null)
                    {
                        status = "declined";
                    }
                }
                if (status == "approved")
                {
                    allApprovedTransactions.Add(currentTransaction);
                }
                allTransactions.Add(new TransactionResult(currentTransaction.transaction_id, status));
            }

            return (allTransactions,allTransactions2);
        }

        public async Task<bool> SendBatchResultsAsync(List<TransactionResult> transactions)
        {
            string json = JsonSerializer.Serialize(transactions);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Patch, baseUrl + $"transaction-batches/{this.batchId}");
            request.Headers.Add("Session-Id", this.SessionInfo.sessionId);
            request.Headers.Add("Competitor-Id", competitorId);
            request.Headers.Add("Results-Hash", GenerateResultHash(json));
            request.Content = content;

            HttpResponseMessage response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Batch is sent successfully!");
                return true;
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Грешка {response.StatusCode}: {error}");
            }
        }
        private string GenerateResultHash(string json)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
        private bool ControlOfDailyLimitsByCardAndUser(List<Transaction>allTransactions,decimal dailyClientSpendingLimit,decimal dailyStandartCardLimit,decimal dailyPremiumCardLimit,string cardId,string cardType,DateTime transactionDate,string userId)
        {
            DateTime startOfTheDay = transactionDate.Date; //взима в същия ден 00:00
            List<Transaction> transactionsByTheCardInTheLastDay = allTransactions
                    .Where(x => x.card_id == cardId && x.timestamp >= startOfTheDay).ToList();

            decimal sumOfTransactionAmountByCard = transactionsByTheCardInTheLastDay.Sum(x => x.amount);

            if (cardType == "standard")
            {
                if (sumOfTransactionAmountByCard > dailyStandartCardLimit)
                {
                    return false;
                }

            }
            else if (cardType == "premium")
            {
                if (sumOfTransactionAmountByCard > dailyPremiumCardLimit)
                {
                    return false;
                }
            }

            List<Transaction> transactionsByUserInTheLastDay = allTransactions.
                Where(x => x.client_id == userId && x.timestamp >= startOfTheDay).ToList();
            decimal sumOfTransactionAmmountByUser = transactionsByUserInTheLastDay.Sum(x => x.amount);

            if (sumOfTransactionAmmountByUser > dailyClientSpendingLimit)
            {
                return false;
            }
            return true;
        }

        private bool ControlOfWeeklyLimitsByCardAndUser(List<Transaction> аllTransactions,decimal weeklyClientSpendingLimit,decimal weeklyStandartCardLimit,decimal weeklyPremiumCardLimit,string cardId,string cardType,DateTime transactionDate,string userid)
        {
            DateTime startOfTheWeek = transactionDate.AddDays(-1*(7 + (transactionDate.DayOfWeek - DayOfWeek.Monday)) % 7).Date;

            List<Transaction> transactionsByTheCardInTheLastWeek = аllTransactions
                  .Where(x => x.card_id == cardId && x.timestamp >= startOfTheWeek).ToList();

            decimal sumOfTransactionAmmountByCard= transactionsByTheCardInTheLastWeek.Sum(x => x.amount);

            if (cardType == "standard")
            {
                if (sumOfTransactionAmmountByCard >weeklyStandartCardLimit)
                {
                    return false;
                }

            }
            else if (cardType == "premium")
            {
                if (sumOfTransactionAmmountByCard > weeklyPremiumCardLimit)
                {
                    return false;
                }
            }
            List<Transaction> transactionsByUserInTheLastWeek = аllTransactions.
               Where(x => x.client_id == userid && x.timestamp >= startOfTheWeek).ToList();

            decimal sumOfTransactionAmmountByUser = transactionsByUserInTheLastWeek.Sum(x => x.amount);

            if (sumOfTransactionAmmountByUser > weeklyClientSpendingLimit)
            {
                return false;
            }

            return true;
        }

        private bool ControlOf10secWindow(List<Transaction>allTransactions,DateTime transactionDate,int transactionsPer10secWindowLimit,string clientId)
        {
            int transactionsInTheLast10sec = allTransactions.Where(x => x.timestamp >= transactionDate.AddSeconds(-10)&&x.client_id==clientId).Count();
            if(transactionsInTheLast10sec >= transactionsPer10secWindowLimit)
            {
                return false;
            }
            return true;
        }
    }
}
