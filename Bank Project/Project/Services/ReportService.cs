using BankingCompetition.Models;
using Project.Models;
using Project.Models.SessionConstraints;
using System.Text;
using System.Text.Json;

namespace BankingCompetition.Services
{
    public class ReportService : IReportService
    {
        private readonly HttpClient client = new HttpClient();
        private readonly List<Transaction> allTransactions;

        private SessionInfo sessionInfo;

        public ReportService(SessionInfo sessionInfo, List<Transaction> allApprovedTransactions)
        {
            this.sessionInfo = sessionInfo;
            this.allTransactions = allApprovedTransactions;
        }


        public async Task<List<ReportConfig>> GetReportConfigurationAsync()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Project.Common.Constants.baseUrl + "report-configuration");
            request.Headers.Add("Session-Id", sessionInfo.sessionId);
            request.Headers.Add("Competitor-Id", Project.Common.Constants.competitorId);

            HttpResponseMessage response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Cound not get configuration report! " + response.StatusCode);
            }

            string json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<List<ReportConfig>>(json, options);
        }

        public List<Report> GenerateReports(List<ReportConfig> configs)
        {
            var reports = new List<Report>();
            decimal feeRate = (decimal)(sessionInfo.spendingLimits.interchangeFeePercentage / 100.0);

            foreach (var config in configs)
            {
                
                List<Transaction> periodTxs = allTransactions.Where(tx =>
                    tx.timestamp >= config.FromTimestamp &&
                    tx.timestamp <= config.ToTimestamp &&
                    (config.ClientIds.Count == 0 || config.ClientIds.Contains(tx.client_id)) &&
                    tx.type == "authorization" 
                ).ToList();

                List<ClientReport> clientReports = new List<ClientReport>();

                var grouped = periodTxs.GroupBy(x => x.client_id);

                foreach (var group in grouped)
                {
                    var approved = group.Where(x => x.status == "approved").ToList();
                    var declined = group.Where(x => x.status == "declined").ToList();

                    clientReports.Add(new ClientReport
                    {
                        clientId = group.Key,
                        totalApprovedCount = approved.Count,
                        totalApprovedAmount = approved.Sum(x => x.amount),
                        totalDecliendCount = declined.Count,
                        totalDeclinedAmount = declined.Sum(x => x.amount),
                        totalEarningsAmount = Math.Round(approved.Sum(x => x.amount) * feeRate, 2)
                    });
                }

                reports.Add(new Report
                {
                    id = config.Id,
                    fromTime = config.FromTimestamp,
                    toTime = config.ToTimestamp,
                    totalApprovedCount = clientReports.Sum(x => x.totalApprovedCount),
                    totalApprovedAmount = clientReports.Sum(x => x.totalApprovedAmount),
                    totalDeclinedCount = clientReports.Sum(x => x.totalDecliendCount),
                    totalDeclinedAmount = clientReports.Sum(x => x.totalDeclinedAmount),
                    totalEarningsAmount = clientReports.Sum(x => x.totalEarningsAmount),
                    clients = clientReports
                });
            }

            return reports;
        }


        public async Task<bool> SendReportsAsync(List<Report> reports)
        {
            string json = JsonSerializer.Serialize(reports);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, Project.Common.Constants.baseUrl + "reports");
            request.Headers.Add("Session-Id", sessionInfo.sessionId);
            request.Headers.Add("Competitor-Id", Project.Common.Constants.competitorId);
            request.Content = content;

            var response = await client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
