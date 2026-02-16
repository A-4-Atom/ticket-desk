using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TicketPlatform.Services
{
    public class ApiClient : IApiClient
    {
        private readonly string _backendBaseUrl;
        private readonly HttpClient _httpClient;

        public ApiClient()
        {
            _backendBaseUrl = ConfigurationManager.AppSettings["BackendBaseUrl"] ?? string.Empty;
            _httpClient = new HttpClient();
        }

        private string BuildUrl(string relativeUrl)
        {
            if (string.IsNullOrEmpty(_backendBaseUrl))
            {
                throw new InvalidOperationException("BackendBaseUrl is not configured.");
            }

            if (!relativeUrl.StartsWith("/"))
            {
                relativeUrl = "/" + relativeUrl;
            }

            return _backendBaseUrl.TrimEnd('/') + relativeUrl;
        }

        public Task<HttpResponseMessage> GetAsync(string relativeUrl)
        {
            var url = BuildUrl(relativeUrl);
            return _httpClient.GetAsync(url);
        }

        public Task<HttpResponseMessage> PostJsonAsync(string relativeUrl, object payload)
        {
            var url = BuildUrl(relativeUrl);
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return _httpClient.PostAsync(url, content);
        }

        public Task<HttpResponseMessage> PostMultipartAsync(string relativeUrl, MultipartFormDataContent content)
        {
            var url = BuildUrl(relativeUrl);
            return _httpClient.PostAsync(url, content);
        }
    }
}
