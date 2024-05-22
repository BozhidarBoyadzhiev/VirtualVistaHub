using System.Linq;
using System.Web.Mvc;
using VirtualVistaHub.Models;
using VirtualVistaHub.Filters;
using System.Data.Entity;

namespace VirtualVistaHub.Controllers
{
    public class HomeController : Controller
    {
        readonly VirtualVistaBaseEntities db = new VirtualVistaBaseEntities();

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult Unauthorized()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        [AllowAnonymous]
        public ActionResult AboutUs()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Contacts()
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
            Session["userLevel"] = "none";
            return RedirectToAction("Index", "Home");
        }

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
                    bool isDeleted = checkLogin.Deleted;
                    if (!isDeleted)
                    {
                        Session["idUser"] = checkLogin.UserId.ToString();
                        Session["UserFirstName"] = checkLogin.FirstName;
                        Session["UserEmail"] = checkLogin.Email;

                        var assignUserLevel = db.Staffs.FirstOrDefault(x => x.UserId.Equals(checkLogin.UserId));

                        if (assignUserLevel == null)
                            Session["userLevel"] = "none";
                        else
                            Session["userLevel"] = assignUserLevel.UserLevel;

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.Notification = "Този акаунт е изтрит";
                    }
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

        [SessionAuthorize("superadmin", "admin", "none")]
        public ActionResult EditUser(int userId)
        {
            var user = db.Users.FirstOrDefault(p => p.UserId == userId);

            if (user == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var currentUserId = Session["idUser"].ToString();
            var currentUserLevel = Session["userLevel"].ToString();

            if (currentUserLevel == "superadmin")
            {
                return View(user);
            }

            if (currentUserLevel == "admin")
            {
                var targetStaff = db.Staffs.FirstOrDefault(u => u.UserId == userId);
                if (userId.ToString() == currentUserId || (targetStaff == null || (targetStaff.UserLevel != "admin" && targetStaff.UserLevel != "superadmin")))
                {
                    return View(user);
                }
                else
                {
                    return RedirectToAction("Users", "Staff");
                }
            }

            if (currentUserLevel == "none" && userId.ToString() == currentUserId)
            {
                return View(user);
            }

            return RedirectToAction("Unauthorized", "Home");
        }


        [HttpGet]
        [SessionAuthorize("superadmin", "admin", "none")]
        public ActionResult DeleteUser(int userId)
        {
            var user = db.Users.FirstOrDefault(p => p.UserId == userId);

            if (user == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            if (user != null)
            {
                if (Session["idUser"].ToString() == user.UserId.ToString() || Session["userLevel"].ToString() != "none")
                {
                    if (user.Deleted == true)
                    {
                        TempData["AlreadyDeleted"] = true;
                    }
                    else
                    {
                        user.Deleted = true;
                        db.Entry(user).State = EntityState.Modified;
                        db.SaveChanges();

                        if (Session["userLevel"].ToString() == "none")
                        {
                            return RedirectToAction("Logout", "Home");
                        }
                    }
                }
            }

            return RedirectToAction("Users", "Staff");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize("superadmin", "admin", "none")]
        public ActionResult EditUser(User user)
        {
            var existingUser = db.Users.Find(user.UserId);

            if (!string.IsNullOrWhiteSpace(user.Password) && user.Password != existingUser.Password)
            {
                existingUser.Password = HashPassword(user.Password);
            }

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;

            db.Entry(existingUser).State = EntityState.Modified;
            db.SaveChanges();

            if (Session["userLevel"].ToString() != "none")
                return RedirectToAction("Users", "Staff");
            else
                return RedirectToAction("Account", "Home");
        }


    }
}
