namespace TicketPlatform.Models
{
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
    }
}
