using BankingCompetition.Models;
using Project.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankingCompetition.Services
{
    public interface IReportService
    {
        Task<List<ReportConfig>> GetReportConfigurationAsync();
        List<Report> GenerateReports(List<ReportConfig> configs);
        Task<bool> SendReportsAsync(List<Report> reports);
    }
}
