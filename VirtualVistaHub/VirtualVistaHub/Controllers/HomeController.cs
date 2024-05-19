using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VirtualVistaHub.Models;
using BCrypt.Net;

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
        public ActionResult Properties()
        {
            return View();
        }
        public ActionResult Account()
        {
            return View();
        }

        public static string HashPassword(string plainPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainPassword);
        }

        public static bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }

        [HttpPost]
        public ActionResult Signup(User user)
        {
            if (db.Users.Any(x => x.Email == user.Email))
            {
                ViewBag.Notification = "This account has already existed";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    user.Password = HashPassword(user.Password);
                    db.Users.Add(user);
                    db.SaveChanges();

                    Session["idUser"] = user.UserId.ToString();
                    Session["UserFirstName"] = user.FirstName.ToString();
                    Session["UserEmail"] = user.Email.ToString();
                }

                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User user)
        {
            var checkLogin = db.Users.FirstOrDefault(x => x.Email.Equals(user.Email));

            if (checkLogin != null)
            {
                bool verified = VerifyPassword(user.Password, checkLogin.Password);

                if (verified)
                {
                    Session["idUser"] = checkLogin.UserId.ToString();
                    Session["UserFirstName"] = checkLogin.FirstName;
                    Session["UserEmail"] = user.Email;
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.Notification = "Wrong email or password";
            }

            return View();
        }
    }
}