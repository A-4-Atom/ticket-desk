using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using TicketPlatform.Models;

namespace TicketPlatform.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index(int page = 1)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Auth");

            var userId = Session["UserId"].ToString();

            var url = $"http://localhost:7071/api/tickets?userId={userId}&page={page}";

            TicketPageResponse apiResult = null;
            var loadError = false; 

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        apiResult = JsonConvert.DeserializeObject<TicketPageResponse>(json);
                    }
                    else
                    {
                        loadError = true;
                    }
                }
                catch (Exception)
                {
                    loadError = true;
                }
            }

            if (apiResult == null)
            {
                apiResult = new TicketPageResponse
                {
                    page = page,
                    pageSize = 5,
                    tickets = new List<Ticket>(),
                    nextPageToken = null
                };
            }

            ViewBag.Page = apiResult.page;
            ViewBag.PageSize = apiResult.pageSize;

            ViewBag.HasNextPage = !string.IsNullOrEmpty(apiResult.nextPageToken);
            ViewBag.HasPrevPage = page > 1;

            // Expose error flag so the view can render a friendly message
            ViewBag.TicketsLoadError = loadError;

            return View(apiResult.tickets);
        }


        [HttpPost]
        public async Task<ActionResult> CreateTicket(Ticket ticket)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Auth");

            // Get role from session
            var role = (Session["Role"] ?? "User").ToString();

            string rolePrefix = null;

            switch (role)
            {
                case "Sales Rep":
                    rolePrefix = "W";
                    break;

                case "SVP":
                    rolePrefix = "X";
                    break;

                case "IT Manager":
                    rolePrefix = "Y";
                    break;

                default:
                    rolePrefix = null;
                    break;
            }

            ticket.userId = Session["UserId"].ToString();
            ticket.employeeCode = (Session["EmployeeCode"] ?? "001").ToString();
            ticket.role = role;
            ticket.rolePrefix = rolePrefix;

            var apiUrl = "http://localhost:7071/api/tickets";

            // Build multipart/form-data
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

            // Attach uploaded files
            var files = Request?.Files;
            if (files != null)
            {
                var count = Math.Min(files.Count, 5);
                for (int i = 0; i < count; i++)
                {
                    var file = files[i];
                    if (file == null || file.ContentLength <= 0)
                        continue;

                    var streamContent = new StreamContent(file.InputStream);
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
                    formData.Add(streamContent, "attachments", file.FileName);
                }
            }

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(apiUrl, formData);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Ticket creation failed!";
                    return RedirectToAction("Index");
                }
            }

            TempData["Success"] = "Ticket created successfully!";
            return RedirectToAction("Index");
        }

    }
}
