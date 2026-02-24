using System.Collections.Generic;
using System.Threading.Tasks;
using TicketPlatform.Models;

namespace TicketPlatform.Services
{
    public interface ITicketService
    {
		Task<TicketPageResponse> GetTicketsAsync(string userId, string role, int page);
		Task<TicketPageResponse> GetDraftTicketsAsync(string userId, string role, int page);
		Task<bool> CreateTicketAsync(Ticket ticket, IEnumerable<System.Web.HttpPostedFileBase> attachments);
		Task<bool> SubmitDraftTicketAsync(string userId, string ticketId);
		Task<bool> BulkCloseTicketsAsync(IEnumerable<string> ticketIds, string role);
    }
}
