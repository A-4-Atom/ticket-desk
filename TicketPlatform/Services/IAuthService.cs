using System.Threading.Tasks;
using TicketPlatform.Models;

namespace TicketPlatform.Services
{
    public interface IAuthService
    {
        Task<TicketPlatform.Models.LoginResponse> LoginAsync(string email, string password);
        Task<bool> SignUpAsync(string fullName, string email, string password, string role);
    }
}
