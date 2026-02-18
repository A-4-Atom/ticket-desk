using System.Net.Http;
using System.Threading.Tasks;

namespace TicketPlatform.Services
{
    public interface IApiClient
    {
        Task<HttpResponseMessage> GetAsync(string relativeUrl);
        Task<HttpResponseMessage> PostJsonAsync(string relativeUrl, object payload);
		Task<HttpResponseMessage> PutJsonAsync(string relativeUrl, object payload);
        Task<HttpResponseMessage> PostMultipartAsync(string relativeUrl, MultipartFormDataContent content);
    }
}
