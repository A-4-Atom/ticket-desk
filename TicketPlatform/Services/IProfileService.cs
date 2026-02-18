using System.Threading.Tasks;
using System.Web;

namespace TicketPlatform.Services
{
    public interface IProfileService
    {
        Task<bool> UpdateDisplayNameAsync(string email, string name);
		Task<string> UploadProfileImageAsync(string email, HttpPostedFileBase file);
    }
}
