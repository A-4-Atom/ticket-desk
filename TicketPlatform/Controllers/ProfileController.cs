using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TicketPlatform.Models;
using TicketPlatform.Services;

namespace TicketPlatform.Controllers
{
	public class ProfileController : Controller
	{
		private readonly IProfileService _profileService;

		public ProfileController()
			: this(new ProfileService())
		{
		}

		// This constructor is useful for unit testing with a mocked IProfileService
		public ProfileController(IProfileService profileService)
		{
			_profileService = profileService;
		}

		// GET: /Profile
		[HttpGet]
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
				UserName = (Session["UserName"] ?? string.Empty).ToString(),
				Email = (Session["Email"] ?? string.Empty).ToString(),
				ProfileImageUrl = (Session["ProfileImageUrl"] ?? string.Empty).ToString()
			};

			return View(model);
		}

		// POST: /Profile
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Index(ProfileViewModel model)
		{
			if (Session["UserId"] == null)
				return RedirectToAction("Login", "Auth");

			var email = (Session["Email"] ?? string.Empty).ToString();
			var originalName = (Session["UserName"] ?? string.Empty).ToString();

			var hasError = false;

			try
			{
				// Update display name if it changed
				if (!string.IsNullOrWhiteSpace(model.UserName) &&
					!string.Equals(model.UserName, originalName, StringComparison.Ordinal))
				{
					var nameUpdated = await _profileService
						.UpdateDisplayNameAsync(email, model.UserName)
						.ConfigureAwait(false);

					if (!nameUpdated)
					{
						hasError = true;
					}
					else
					{
						Session["UserName"] = model.UserName;
					}
				}

				// Upload profile image if any file provided - field name must match backend expectation
				var file = Request?.Files?["file"] as HttpPostedFileBase;
				if (file != null && file.ContentLength > 0)
				{
					var imageUrl = await _profileService
						.UploadProfileImageAsync(email, file)
						.ConfigureAwait(false);

					if (string.IsNullOrWhiteSpace(imageUrl))
					{
						hasError = true;
					}
					else
					{
						// keep latest profile image url in session so it is available after redirect
						Session["ProfileImageUrl"] = imageUrl;
					}
				}
			}
			catch
			{
				hasError = true;
			}

			// Rebuild the model from session to ensure consistency
			var viewModel = new ProfileViewModel
			{
				UserId = (Session["UserId"] ?? string.Empty).ToString(),
				EmployeeCode = (Session["EmployeeCode"] ?? string.Empty).ToString(),
				Role = (Session["Role"] ?? string.Empty).ToString(),
				RolePrefix = (Session["RolePrefix"] ?? string.Empty).ToString(),
				UserName = (Session["UserName"] ?? string.Empty).ToString(),
				Email = (Session["Email"] ?? string.Empty).ToString(),
				ProfileImageUrl = (Session["ProfileImageUrl"] ?? string.Empty).ToString()
			};

			ViewBag.ProfileUpdateError = hasError;

			return View(viewModel);
		}
	}
}
