using System.Web.Mvc;

namespace TicketPlatform.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // Protect this page
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }
    }
}
