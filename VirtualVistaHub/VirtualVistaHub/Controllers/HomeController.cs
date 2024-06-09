using System.Linq;
using System.Web.Mvc;
using VirtualVistaHub.Models;
using VirtualVistaHub.Filters;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace VirtualVistaHub.Controllers
{
    public class HomeController : Controller
    {
        readonly VirtualVistaBaseEntities db = new VirtualVistaBaseEntities();

        public static string HashPassword(string plainPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainPassword);
        }

        public static bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }


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
            return View();
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
        public ActionResult Signup()
        {
            return View();
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

        [AllowAnonymous]
        public ActionResult Search(string typeOfProperty, string district, string neighbourhood, string typeOfConstruction, string typeOfSale)
        {
            var properties = db.Properties.Where(p => p.ApprovalStatus.ToString() != "Not Approved" && p.Sold != true);

            if (!string.IsNullOrEmpty(typeOfProperty))
            {
                properties = properties.Where(p => p.TypeOfProperty == typeOfProperty);
            }
            if (!string.IsNullOrEmpty(district))
            {
                properties = properties.Where(p => p.District == district);
            }
            if (!string.IsNullOrEmpty(neighbourhood))
            {
                properties = properties.Where(p => p.Neighbourhood == neighbourhood);
            }
            if (!string.IsNullOrEmpty(typeOfConstruction))
            {
                properties = properties.Where(p => p.TypeOfConstruction == typeOfConstruction);
            }
            if (!string.IsNullOrEmpty(typeOfSale))
            {
                properties = properties.Where(p => p.TypeOfSale == typeOfSale);
            }
            var propertyList = properties.ToList();

            var propertyDetails = new Dictionary<int, (string TableName, string FirstImagePath)>();

            foreach (var prop in propertyList)
            {
                string query = $"SELECT TOP 1 Images FROM {prop.PropertyDetailsTable}";
                var image = db.Database.SqlQuery<string>(query).FirstOrDefault();
                string firstImagePath = image ?? string.Empty;
                propertyDetails.Add(prop.PropertyId, (prop.PropertyDetailsTable, firstImagePath));
            }

            var viewModel = new PropertySearchViewModel
            {
                Properties = propertyList,
                TypeOfProperty = typeOfProperty,
                District = district,
                Neighbourhood = neighbourhood,
                TypeOfConstruction = typeOfConstruction,
                TypeOfSale = typeOfSale,
                PropertyDetails = propertyDetails
            };

            return View(viewModel);
        }

        [SessionAuthorize]
        public ActionResult Properties()
        {
            var userId = Session["idUser"];

            var properties = db.Properties.Where(p => p.UserId.ToString() == userId.ToString() && p.ApprovalStatus.ToString() == "Approved").ToList();
            return View(properties);
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

        [HttpGet]
        [SessionAuthorize]
        public ActionResult Account(int userId)
        {
            var user = db.Users.FirstOrDefault(p => p.UserId == userId);

            if (user == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var currentUserId = Session["idUser"].ToString();


            if (userId.ToString() == currentUserId)
            {
                return View(user);
            }

            return RedirectToAction("Unauthorized", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize]
        public ActionResult Account(User user)
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

            return RedirectToAction("Account", "Home", new {userId = Session["idUser"] });
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
    }
}
