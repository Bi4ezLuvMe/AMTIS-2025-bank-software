using BankingCompetition.Models;
using BankingCompetition.Services;
using BankingCompetition.Services;
using BankingCompetition.Utils;
using Project.Models;
using Project.Models.SessionConstraints;
using Project.Services.PostRequests;
using System;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static readonly string baseUrl = "https://competition-engine-production.up.railway.app/";
    static readonly string competitorId = "ea04a325-4362-11f0-bc60-0242ac130003";
    static readonly string gitSha = "4576b7acef33d2b64100da269986c9c916b50f12";      
    static readonly string sessionType = "test";
    static HttpClient client = new HttpClient();

    static async Task Main()
    {
        SessionInfo sessionInfo = await GetSessionLimits();

        TransactionBatch transactionsToAudit = await GetTransactionsForAudit(sessionInfo);
    }

    private static async Task<TransactionBatch?> GetTransactionsForAudit(SessionInfo sessionInfo)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, baseUrl + "transaction-batches");
        request.Headers.Add("Session-Id", sessionInfo.sessionId);
        request.Headers.Add("Competitor-Id", competitorId);

        var response = await client.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        TransactionBatch transactionsToAudit = JsonSerializer.Deserialize<TransactionBatch>(responseContent);
        return transactionsToAudit;
    }

    private static async Task<SessionInfo?> GetSessionLimits()
    {
        var postData = new PostData(competitorId, sessionType, gitSha);

        var json = JsonSerializer.Serialize(postData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(baseUrl + "sessions", content);
        var responseContent = response.Content.ReadAsStringAsync().Result;

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine(responseContent);
        }
        else
        {
            throw new Exception("Error: " + response.StatusCode);
        }
        SessionInfo? sessionInfo = JsonSerializer.Deserialize<SessionInfo>(responseContent);
        return sessionInfo;
    }
}
