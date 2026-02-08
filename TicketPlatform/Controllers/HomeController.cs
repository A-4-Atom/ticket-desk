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
    }
}
