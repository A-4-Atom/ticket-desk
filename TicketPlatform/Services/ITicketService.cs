using System.Collections.Generic;
using System.Threading.Tasks;
using TicketPlatform.Models;

namespace TicketPlatform.Services
{
    public interface ITicketService
    {
        Task<TicketPageResponse> GetTicketsAsync(string userId, int page);
        Task<bool> CreateTicketAsync(Ticket ticket, IEnumerable<System.Web.HttpPostedFileBase> attachments);
    }
}
