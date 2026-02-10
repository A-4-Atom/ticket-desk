using System.Collections.Generic;

namespace TicketPlatform.Models
{
    public class TicketPageResponse
    {
        public int page { get; set; }
        public int pageSize { get; set; }
        public List<Ticket> tickets { get; set; }
        public string nextPageToken { get; set; }
    }
}
