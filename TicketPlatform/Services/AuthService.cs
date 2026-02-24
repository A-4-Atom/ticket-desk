using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TicketPlatform.Models;

namespace TicketPlatform.Services
{
    public class AuthService : IAuthService
    {
        private readonly IApiClient _apiClient;

        public AuthService() : this(new ApiClient())
        {
        }

        public AuthService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<TicketPlatform.Models.LoginResponse> LoginAsync(string email, string password)
        {
            var payload = new
            {
                email,
                password
            };

            var response = await _apiClient.PostJsonAsync("/api/auth/login", payload).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var user = JsonConvert.DeserializeObject<LoginResponse>(json);
            return user;
        }

        public async Task<bool> SignUpAsync(string fullName, string email, string password, string role)
        {
            var payload = new
            {
                name = fullName,
                email,
                password,
                role
            };

            var response = await _apiClient.PostJsonAsync("/api/auth/signup", payload).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }
    }
}
