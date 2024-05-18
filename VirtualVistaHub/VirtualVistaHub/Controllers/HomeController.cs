using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VirtualVistaHub.Models;

namespace VirtualVistaHub.Controllers
{
    public class HomeController : Controller
    {
        VirtualVistaBaseEntities db = new VirtualVistaBaseEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(TBL tBLUserInfo)
        {
            return View();
        }
    }
}