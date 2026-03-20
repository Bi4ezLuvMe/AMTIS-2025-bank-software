namespace Bank_API.Data
{
    public class SessionData
    {
        public string SessionId { get; set; }
        public List<dynamic> Transactions { get; set; } = new();
    }
}
