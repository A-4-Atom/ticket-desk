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

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Auth");

            return View();
        }

        public async Task<ActionResult> MyTickets(int page = 1)
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

            ViewBag.TicketsLoadError = loadError;
			ViewBag.ShowSubmitAction = false;
			ViewBag.PaginationAction = "MyTickets";

            return View("MyTickets", apiResult.tickets);
        }

		public async Task<ActionResult> DraftTickets(int page = 1)
		{
			if (Session["UserId"] == null)
				return RedirectToAction("Login", "Auth");

			var userId = Session["UserId"].ToString();

			var loadError = false;
			TicketPageResponse apiResult = null;

			try
			{
				apiResult = await _ticketService.GetDraftTicketsAsync(userId, page).ConfigureAwait(false);
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
			else
			{
				var tickets = apiResult.tickets ?? new List<Ticket>();
				apiResult.tickets = tickets.FindAll(t =>
					(t != null) &&
					(
						t.isDraft ||
						string.Equals(t.status, "Draft", StringComparison.OrdinalIgnoreCase)
					));
			}

			ViewBag.Page = apiResult.page;
			ViewBag.PageSize = apiResult.pageSize;

			ViewBag.HasNextPage = !string.IsNullOrEmpty(apiResult.nextPageToken);
			ViewBag.HasPrevPage = page > 1;

			ViewBag.TicketsLoadError = loadError;
			ViewBag.ShowSubmitAction = true;
			ViewBag.PaginationAction = "DraftTickets";

			return View("DraftTickets", apiResult.tickets);
		}
		

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateTicket(Ticket ticket)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Auth");

            var roleString = (Session["Role"] ?? "User").ToString();
            var userRole = RolePrefixHelper.ParseRole(roleString);
            var rolePrefix = RolePrefixHelper.GetPrefix(userRole);

            ticket.userId = Session["UserId"].ToString();
            ticket.employeeCode = (Session["EmployeeCode"] ?? "001").ToString();
            ticket.role = roleString;
            ticket.rolePrefix = rolePrefix;

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
			
			if (ticket.isDraft)
			{
				TempData["Success"] = "Ticket saved as draft.";
				return RedirectToAction("DraftTickets");
			}
			
			TempData["Success"] = "Ticket created successfully!";
			return RedirectToAction("Index");
        }
		
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SubmitDraftTicket(string ticketId)
		{
			if (Session["UserId"] == null)
				return RedirectToAction("Login", "Auth");

			if (string.IsNullOrWhiteSpace(ticketId))
			{
				TempData["Error"] = "Invalid ticket identifier.";
				return RedirectToAction("DraftTickets");
			}

			var userId = Session["UserId"].ToString();
			var success = false;
			try
			{
				success = await _ticketService.SubmitDraftTicketAsync(userId, ticketId).ConfigureAwait(false);
			}
			catch (Exception)
			{
				success = false;
			}

			if (!success)
			{
				TempData["Error"] = "Submitting draft ticket failed!";
			}
			else
			{
				TempData["Success"] = "Draft ticket submitted successfully!";
			}

			return RedirectToAction("DraftTickets");
		}
    }
}
