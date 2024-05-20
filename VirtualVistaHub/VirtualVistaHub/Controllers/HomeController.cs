using System.Linq;
using System.Web.Mvc;
using VirtualVistaHub.Models;
using VirtualVistaHub.Filters;

namespace VirtualVistaHub.Controllers
{
    public class HomeController : Controller
    {
        VirtualVistaBaseEntities db = new VirtualVistaBaseEntities();

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        [AllowAnonymous]
        public ActionResult Buy()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult SellRent()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Search()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Signup()
        {
            return View();
        }

        [SessionAuthorize]
        public ActionResult Properties()
        {
            var userId = Session["idUser"];
            if (userId == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var properties = db.Properties.Where(p => p.UserId.ToString() == userId.ToString()).ToList();
            return View(properties);
        }

        [SessionAuthorize]
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
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Signup(User user)
        {
            if (db.Users.Any(x => x.Email == user.Email))
            {
                ViewBag.Notification = "This account already exists";
                return View();
            }
            else
            {
                if (ModelState.IsValid && user.Password == user.RePassword)
                {
                    user.Password = HashPassword(user.Password);
                    db.Users.Add(user);
                    db.SaveChanges();

                    Session["idUser"] = user.UserId.ToString();
                    Session["UserFirstName"] = user.FirstName;
                    Session["UserEmail"] = user.Email;

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Notification = "The password and confirmation password do not match.";
                    return View();
                }
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session["userLevel"] = "None";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
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
                    Session["UserEmail"] = checkLogin.Email;

                    var assignUserLevel = db.Staffs.FirstOrDefault(x => x.UserId.Equals(checkLogin.UserId));

                    if (assignUserLevel == null)
                        Session["userLevel"] = "None";
                    else
                        Session["userLevel"] = assignUserLevel.UserLevel;

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Notification = "Wrong email or password";
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
