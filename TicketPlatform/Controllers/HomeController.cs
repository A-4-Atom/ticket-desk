using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using TicketPlatform.Models;
using TicketPlatform.Services;

namespace TicketPlatform.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITicketService _ticketService;

        public HomeController()
            : this(new TicketService())
        {
        }

        // This constructor is useful for unit testing with a mocked ITicketService
        public HomeController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public async Task<ActionResult> Index(int page = 1)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Auth");

            var userId = Session["UserId"].ToString();

            var loadError = false;
            TicketPageResponse apiResult = null;

            try
            {
                apiResult = await _ticketService.GetTicketsAsync(userId, page).ConfigureAwait(false);
                if (apiResult == null)
                {
                    loadError = true;
                }
            }
            catch (Exception)
            {
                loadError = true;
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateTicket(Ticket ticket)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Auth");

            // Get role from session and resolve role prefix via helper
            var roleString = (Session["Role"] ?? "User").ToString();
            var userRole = RolePrefixHelper.ParseRole(roleString);
            var rolePrefix = RolePrefixHelper.GetPrefix(userRole);

            ticket.userId = Session["UserId"].ToString();
            ticket.employeeCode = (Session["EmployeeCode"] ?? "001").ToString();
            ticket.role = roleString;
            ticket.rolePrefix = rolePrefix;

            // Collect uploaded files for the service layer
            var files = Request?.Files;
            var attachments = new List<System.Web.HttpPostedFileBase>();
            if (files != null)
            {
                var count = Math.Min(files.Count, 5);
                for (int i = 0; i < count; i++)
                {
                    var file = files[i];
                    if (file == null || file.ContentLength <= 0)
                        continue;

                    attachments.Add(file);
                }
            }

            var success = false;
            try
            {
                success = await _ticketService.CreateTicketAsync(ticket, attachments).ConfigureAwait(false);
            }
            catch (Exception)
            {
                success = false;
            }

            if (!success)
            {
                TempData["Error"] = "Ticket creation failed!";
                return RedirectToAction("Index");
            }

            TempData["Success"] = "Ticket created successfully!";
            return RedirectToAction("Index");
        }

    }
}
