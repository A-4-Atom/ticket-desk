using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace TicketPlatform.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IApiClient _apiClient;

        public ProfileService() : this(new ApiClient())
        {
        }

        public ProfileService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<bool> UpdateDisplayNameAsync(string email, string name)
        {
            var payload = new
            {
                name
            };

			var relativeUrl = $"/api/users/{email}/update-name";
			var response = await _apiClient.PutJsonAsync(relativeUrl, payload).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }

		public async Task<string> UploadProfileImageAsync(string email, HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength <= 0)
				return string.Empty;

			var formData = new MultipartFormDataContent();
			var streamContent = new StreamContent(file.InputStream);
			streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType ?? "image/jpeg");
			formData.Add(streamContent, "file", file.FileName);

			var relativeUrl = $"/api/users/{email}/upload-profile";
			var response = await _apiClient.PostMultipartAsync(relativeUrl, formData).ConfigureAwait(false);
			if (!response.IsSuccessStatusCode)
			{
				return string.Empty;
			}

			var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			return content ?? string.Empty;
        }
    }
}
