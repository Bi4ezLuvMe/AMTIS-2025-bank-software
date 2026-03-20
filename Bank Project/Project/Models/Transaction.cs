namespace BankingCompetition.Models
{
    public class Transaction
    {
        public string transaction_id { get; set; }
        public string card_id { get; set; }
        public string card_type { get; set; }
        public string client_id { get; set; }
        public decimal amount { get; set; }
        public DateTime timestamp { get; set; }
        public string type { get; set; }
    }
}
