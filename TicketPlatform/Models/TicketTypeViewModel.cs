using System;

namespace TicketPlatform.Models
{
    public class TicketTypeViewModel
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Category { get; set; }
        public bool RequiresConfidentiality { get; set; }
    }
}
