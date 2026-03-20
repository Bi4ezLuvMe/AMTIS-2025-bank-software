using System.Threading.Tasks;

namespace BankingCompetition.Services
{
    public interface ISessionService
    {
        Task<bool> InitializeSessionAsync();
        string SessionId { get; }
    }
}
