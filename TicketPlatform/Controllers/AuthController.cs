using System;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Newtonsoft.Json;
using System.Threading.Tasks;
using TicketPlatform.Services;
using TicketPlatform.Models;

namespace TicketPlatform.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController()
            : this(new AuthService())
        {
        }

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // GET: /Auth/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        public async Task<ActionResult> Login(string email, string password)
        {
            try
            {
                var user = await _authService.LoginAsync(email, password).ConfigureAwait(false);

                if (user == null)
                {
                    TempData["Error"] = "Invalid email or password.";
                    return RedirectToAction("Login");
                }

                Session["UserId"] = user.userId;
                Session["EmployeeCode"] = user.employeeCode;
                Session["Role"] = user.role;
                Session["RolePrefix"] = user.rolePrefix;
                Session["UserName"] = user.name;
                Session["Email"] = user.email;
                Session["ProfileImageUrl"] = user.profileImageUrl;

                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                TempData["Error"] = "Something went wrong. Please try again.";
                return RedirectToAction("Login");
            }
        }

        // GET: /Auth/SignUp
        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login");
        }
        [HttpPost]
        public async Task<ActionResult> SignUp(string fullName, string email, string password, string role)
        {
            try
            {
                var success = await _authService.SignUpAsync(fullName, email, password, role).ConfigureAwait(false);

                if (!success)
                {
                    TempData["Error"] = "Signup failed. Please check details or try another email.";
                    return RedirectToAction("SignUp");
                }

                TempData["Success"] = "Account created successfully! Please login.";
                return RedirectToAction("Login");
            }
            catch (Exception)
            {
                TempData["Error"] = "Something went wrong. Please try again.";
                return RedirectToAction("SignUp");
            }
        }

    }

}
