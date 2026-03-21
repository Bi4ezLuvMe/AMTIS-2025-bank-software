using BankingCompetition.Services;
using Project.Models;
using Project.Models.SessionConstraints;

class Program
{
    static async Task Main()
    {
        SessionService session = new SessionService();

        SessionInfo sessionInfo = await session.InitializeSessionAsync();

        TransactionService transactionService = new TransactionService(sessionInfo);

        while (true)
        {
            TransactionBatch batch = await transactionService.GetTransactionBatchesAsync();

            if (batch==null||batch.transactions == null || batch.transactions.Count == 0)
            {
                Console.WriteLine("All Transactions are processed!");
                break;
            }

            List<TransactionResult> processedTransactions = await transactionService.ProcessTransactions(batch.transactions);

            bool sendBatchResults = await transactionService.SendBatchResultsAsync(processedTransactions);
        }
        ;

    }
}
