using BankingCompetition.Services;
using Project.Models;
using Project.Models.SessionConstraints;
using System.Text.Json;

class Program
{
    static async Task Main()
    {
        SessionService session = new SessionService();

        await session.InitializeSessionAsync();

        TransactionService transactionService = new TransactionService(session.SessionId);

        await transactionService.GetTransactionBatchesAsync();
    }
}
