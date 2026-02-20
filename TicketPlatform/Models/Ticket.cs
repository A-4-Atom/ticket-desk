using System.Collections.Generic;

namespace TicketPlatform.Models

{
    public class TicketAttachment
    {
        public string fileName { get; set; }
        public string fileType { get; set; }
        public string fileUrl { get; set; }
        public string uploadedAt { get; set; }
    }
    public class Ticket
    {
        public string id { get; set; }

        public string userId { get; set; }
        public string employeeCode { get; set; }
        public string role { get; set; }
        public string rolePrefix { get; set; }

        public string title { get; set; }
        public string description { get; set; }
        public string category { get; set; }

        public string status { get; set; }
        public string createdAt { get; set; }
        public string confirmationNumber { get; set; }

		// Indicates whether this ticket is currently a draft
		public bool isDraft { get; set; }

		public List<TicketAttachment> attachments { get; set; }
    }
}
