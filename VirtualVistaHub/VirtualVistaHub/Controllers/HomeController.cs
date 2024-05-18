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
            return View(db.Users.ToList());
        }

        public ActionResult Buy()
        {
            return View();
        }
        public ActionResult SellRent()
        {
            return View();
        }
        public ActionResult Search()
        {
            return View();
        }

        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(User tBLUserInfo)
        {
            if (db.Users.Any(x => x.Email == tBLUserInfo.Email))
            {
                ViewBag.Notification = "This account has already existed";
                return View();
            }
            else
            {
                db.Users.Add(tBLUserInfo);
                db.SaveChanges();

                Session["idUser"] = tBLUserInfo.UserId.ToString();
                Session["UserFirstName"] = tBLUserInfo.FirstName.ToString();
                Session["UserEmail"] = tBLUserInfo.Email.ToString();

                return RedirectToAction("Index", "Home");
            }
        }
    }
}