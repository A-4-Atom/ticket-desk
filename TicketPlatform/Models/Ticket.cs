using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TicketPlatform.Models
{
    public class Ticket
    {
        public string id { get; set; }
        public string title { get; set; }
        public string category { get; set; }
        public string status { get; set; }
        public string createdAt { get; set; }
    }
}
