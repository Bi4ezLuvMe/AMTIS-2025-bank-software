using BankingCompetition.Models;
using Project.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankingCompetition.Services
{
    public interface IReportService
    {
        Task<List<ReportConfig>> GetReportConfigurationAsync(string sessionId, string competitorId);
        List<Report> GenerateReports(List<ReportConfig> configs);
        Task<bool> SendReportsAsync(string sessionId, string competitorId, List<Report> reports);
    }
}
