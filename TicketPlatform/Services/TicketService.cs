using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TicketPlatform.Models;

namespace TicketPlatform.Services
{
    public class TicketService : ITicketService
    {
        private readonly IApiClient _apiClient;

        public TicketService() : this(new ApiClient())
        {
        }

        public TicketService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<TicketPageResponse> GetTicketsAsync(string userId, int page)
        {
            var relativeUrl = $"/api/tickets?userId={userId}&page={page}";
            var response = await _apiClient.GetAsync(relativeUrl).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiResult = JsonConvert.DeserializeObject<TicketPageResponse>(json);
            return apiResult;
        }

        public async Task<bool> CreateTicketAsync(Ticket ticket, IEnumerable<System.Web.HttpPostedFileBase> attachments)
        {
            var formData = new MultipartFormDataContent();

            formData.Add(new StringContent(ticket.userId ?? string.Empty), "userId");
            formData.Add(new StringContent(ticket.employeeCode ?? string.Empty), "employeeCode");
            formData.Add(new StringContent(ticket.role ?? string.Empty), "role");
            if (!string.IsNullOrEmpty(ticket.rolePrefix))
            {
                formData.Add(new StringContent(ticket.rolePrefix), "rolePrefix");
            }
            formData.Add(new StringContent(ticket.title ?? string.Empty), "title");
            formData.Add(new StringContent(ticket.description ?? string.Empty), "description");
            formData.Add(new StringContent(ticket.category ?? string.Empty), "category");
			formData.Add(new StringContent(ticket.isDraft ? "true" : "false"), "isDraft");

            if (attachments != null)
            {
                foreach (var file in attachments)
                {
                    if (file == null || file.ContentLength <= 0)
                        continue;

                    var streamContent = new StreamContent(file.InputStream);
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
                    formData.Add(streamContent, "attachments", file.FileName);
                }
            }

            var response = await _apiClient.PostMultipartAsync("/api/tickets", formData).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }

		public async Task<TicketPageResponse> GetDraftTicketsAsync(string userId, int page)
		{
			var relativeUrl = $"/api/tickets?userId={userId}&page={page}&isDraft=true";
			var response = await _apiClient.GetAsync(relativeUrl).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				return null;
			}

			var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var apiResult = JsonConvert.DeserializeObject<TicketPageResponse>(json);
			return apiResult;
		}

		public async Task<bool> SubmitDraftTicketAsync(string userId, string ticketId)
		{
			var relativeUrl = $"/api/tickets/{userId}/{ticketId}/submit";
			var response = await _apiClient.PostJsonAsync(relativeUrl, new { }).ConfigureAwait(false);
			return response.IsSuccessStatusCode;
		}
    }
}
