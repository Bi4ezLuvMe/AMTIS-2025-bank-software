using Bank_API.Data;
using Bank_API.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bank_API.Web.Controllers
{
    [ApiController]
    [Route("")]
    public class BankApiController : Controller
    {
        [HttpPost("sessions")]
        public IActionResult StartSession([FromBody] dynamic request)
        {
            string sessionId = Guid.NewGuid().ToString();
            var transactions = CompetitionEngineService.GenerateMockTransactions(100); // Генерираме 100 за проба

            DataStore.Sessions[sessionId] = new SessionData
            {
                SessionId = sessionId,
                Transactions = transactions
            };

            return Ok(new
            {
                sessionId = sessionId,
                limits = new
                {
                    standard = new { daily = 150, weekly = 800 },
                    premium = new { daily = 300, weekly = 2000 },
                    user = new { daily = 400, weekly = 3000 }
                },
                tax_percent = 0.3
            });
        }
        [HttpGet("transaction-batches")]
        public IActionResult GetBatch([FromHeader(Name = "Session-Id")] string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId) || !DataStore.Sessions.ContainsKey(sessionId))
                return Unauthorized("Invalid Session-Id");

            var session = DataStore.Sessions[sessionId];

            // Взимаме първите 10 транзакции и ги махаме от списъка (симулираме бач)
            var batch = session.Transactions.Take(10).ToList();
            session.Transactions.RemoveRange(0, batch.Count);

            return Ok(new
            {
                transactionsBatchId = Guid.NewGuid().ToString(),
                transactions = batch
            });
        }
        [HttpPatch("transaction-batches/{id}")]
        public IActionResult AcceptResults(string id, [FromBody] dynamic results)
        {
            // Тук просто казваме "OK", за да можеш да си тестваш клиента
            return Ok(new { message = "Results received successfully" });
        }
    }
}
