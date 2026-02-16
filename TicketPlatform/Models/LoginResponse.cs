namespace TicketPlatform.Models
{
    public class LoginResponse
    {
        public string userId { get; set; }
        public string employeeCode { get; set; }
        public string role { get; set; }
        public string rolePrefix { get; set; }
        public string name { get; set; }
    }
}
