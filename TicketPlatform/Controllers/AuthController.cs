using System;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace TicketPlatform.Controllers
{
    public class AuthController : Controller
    {
        // GET: /Auth/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            try
            {
                // 1) Call Azure Function
                var functionUrl = "http://localhost:7255/api/auth/login";

                using (var client = new HttpClient())
                {
                    var payload = new
                    {
                        email = email,
                        password = password
                    };

                    var json = JsonConvert.SerializeObject(payload);

                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = client.PostAsync(functionUrl, content).Result;

                    // If login failed
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = "Invalid email or password.";
                        return RedirectToAction("Login");
                    }

                    // 2) Read JSON response
                    var responseJson = response.Content.ReadAsStringAsync().Result;

                    var user = JsonConvert.DeserializeObject<LoginResponse>(responseJson);

                    // 3) Store user in SESSION (server-side)
                    Session["UserId"] = user.userId;
                    Session["EmployeeCode"] = user.employeeCode;
                    Session["Role"] = user.role;
                    Session["RolePrefix"] = user.rolePrefix;
                    Session["UserName"] = user.name;

                    // 4) Redirect after successful login
                    return RedirectToAction("Index", "Home");
                }
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
        public ActionResult SignUp(string fullName, string email, string password, string role)
        {
            try
            {
                var functionUrl = "http://localhost:7255/api/auth/signup";

                using (var client = new HttpClient())
                {
                    var payload = new
                    {
                        name = fullName,
                        email = email,
                        password = password,
                        role = role
                    };

                    var json = JsonConvert.SerializeObject(payload);

                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = client.PostAsync(functionUrl, content).Result;

                    // If signup failed
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = "Signup failed. Please check details or try another email.";
                        return RedirectToAction("SignUp");
                    }

                    // Success → redirect to login
                    TempData["Success"] = "Account created successfully! Please login.";
                    return RedirectToAction("Login");
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Something went wrong. Please try again.";
                return RedirectToAction("SignUp");
            }
        }

    }

    // This class should match the Azure Function JSON response
    public class LoginResponse
    {
        public string userId { get; set; }
        public string employeeCode { get; set; }
        public string role { get; set; }
        public string rolePrefix { get; set; }
        public string name { get; set; }
    }
}
