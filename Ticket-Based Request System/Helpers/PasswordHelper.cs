using System.Security.Cryptography;
using System.Text;

namespace Ticket_Based_Request_System.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            try
            {
                using var sha = SHA256.Create();
                var bytes = Encoding.UTF8.GetBytes(password);
                return Convert.ToBase64String(sha.ComputeHash(bytes));
            }
            catch
            {
                throw;
            }
        }

        public static bool Verify(string password, string hash)
        {
            try
            {
                return HashPassword(password) == hash;
            }
            catch
            {
                throw;
            }
        }
    }
}
