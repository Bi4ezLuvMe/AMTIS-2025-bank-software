using Bank_API.Data;
using System.Collections.Concurrent;

namespace Bank_API.Web.Services
{
    public static class CompetitionEngineService
    {
        public static List<dynamic> GenerateMockTransactions(int count)
        {
            var list = new List<dynamic>();
            for (int i = 0; i < count; i++)
            {
                list.Add(new
                {
                    transaction_id = Guid.NewGuid().ToString(),
                    card_id = "card_" + (i % 5),
                    card_type = i % 2 == 0 ? "premium" : "standard",
                    client_id = "client_" + (i % 3),
                    amount = Math.Round(new Random().NextDouble() * 100, 2),
                    event_time = DateTime.UtcNow.AddMinutes(i),
                    transaction_type = "authorization"
                });
            }
            return list;
        }
    }
}
