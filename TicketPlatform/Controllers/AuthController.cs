using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TicketPlatform.Controllers
{
    public class AuthController : Controller
    {
        // GET: Auth
        public ActionResult SignUp()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
    }
}