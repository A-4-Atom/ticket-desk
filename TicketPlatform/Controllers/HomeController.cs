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
        public async Task<ActionResult> Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Auth");

            var userId = Session["UserId"].ToString();

            var url = $"http://localhost:7255/api/tickets?userId={userId}";

            List<Ticket> tickets = new List<Ticket>();
            System.Diagnostics.Debug.WriteLine("SESSION USERID = " + userId);
            System.Diagnostics.Debug.WriteLine("API URL = " + url);


            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    tickets = JsonConvert.DeserializeObject<List<Ticket>>(json);
                }
            }

            return View(tickets);
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

            // Fill required backend fields
            ticket.userId = Session["UserId"].ToString();
            ticket.employeeCode = (Session["EmployeeCode"] ?? "001").ToString();
            ticket.role = role;
            ticket.rolePrefix = rolePrefix;

            var apiUrl = "http://localhost:7255/api/tickets";

            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(ticket);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);

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
