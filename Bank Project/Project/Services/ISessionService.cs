using Project.Models.SessionConstraints;
using System.Threading.Tasks;

namespace BankingCompetition.Services
{
    public interface ISessionService
    {
        Task<SessionInfo> InitializeSessionAsync();
        string SessionId { get; }
    }
}
