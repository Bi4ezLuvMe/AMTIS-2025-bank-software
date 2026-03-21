using System.Threading.Tasks;

namespace BankingCompetition.Services
{
    public interface ISessionService
    {
        Task InitializeSessionAsync();
        string SessionId { get; }
    }
}
