using System.Web.Mvc;
using TicketPlatform.Models;

namespace TicketPlatform.Controllers
{
	public class ProfileController : Controller
	{
		// GET: /Profile
		public ActionResult Index()
		{
			if (Session["UserId"] == null)
				return RedirectToAction("Login", "Auth");

			var model = new ProfileViewModel
			{
				UserId = (Session["UserId"] ?? string.Empty).ToString(),
				EmployeeCode = (Session["EmployeeCode"] ?? string.Empty).ToString(),
				Role = (Session["Role"] ?? string.Empty).ToString(),
				RolePrefix = (Session["RolePrefix"] ?? string.Empty).ToString(),
				UserName = (Session["UserName"] ?? string.Empty).ToString()
			};

			return View(model);
		}
	}
}
