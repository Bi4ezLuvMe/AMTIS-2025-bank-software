using BankingCompetition.Models;
using BankingCompetition.Services;
using Project.Models;
using Project.Models.SessionConstraints;
using System.Collections.Generic;
using System.Transactions;

class Program
{
    static async Task Main()
    {
        SessionService session = new SessionService();

        SessionInfo sessionInfo = await session.InitializeSessionAsync();

        TransactionService transactionService = new TransactionService(sessionInfo);

        List<BankingCompetition.Models.Transaction> allTransactions = new List<BankingCompetition.Models.Transaction>();

        while (true)
        {
            TransactionBatch batch = await transactionService.GetTransactionBatchesAsync();

            if (batch==null||batch.transactions == null || batch.transactions.Count == 0)
            {
                Console.WriteLine("All Transactions are processed!");
                break;
            }

            var transactions = transactionService.ProcessTransactions(batch.transactions);
            List<TransactionResult> processedTransactions = transactions.Result.Item1;

            allTransactions.AddRange(transactions.Result.Item2);

            bool sendBatchResults = await transactionService.SendBatchResultsAsync(processedTransactions);
        }

        ReportService reportService = new ReportService(sessionInfo,allTransactions);

        List<ReportConfig> reportconfig = await reportService.GetReportConfigurationAsync();

        List<Report> reports = reportService.GenerateReports(reportconfig);

        bool sendReports = await reportService.SendReportsAsync(reports);

        ;
    }
}
