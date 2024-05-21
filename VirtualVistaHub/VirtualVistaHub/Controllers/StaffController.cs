using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VirtualVistaHub.Filters;
using VirtualVistaHub.Models;
using System.Data.SqlClient;
using System.Data.Entity.Core.Metadata.Edm;

namespace VirtualVistaHub.Controllers
{
    public class StaffController : Controller
    {
        readonly VirtualVistaBaseEntities db = new VirtualVistaBaseEntities();

        [SessionAuthorize("admin", "superadmin", "moderator")]
        public ActionResult Properties()
        {
            var properties = db.Properties.Include(p => p.User).ToList();
            return View(properties);
        }

        [SessionAuthorize("superadmin")]
        public ActionResult Users()
        {
            var users = db.Users.Include(p => p.Staff).ToList();
            return View(users);
        }

        [SessionAuthorize("superadmin")]
        public ActionResult UserLevel()
        {
            var users = db.Users.Include(p => p.Staff).ToList();
            return View(users);
        }

        [SessionAuthorize("admin", "superadmin", "moderator")]
        public ActionResult ApproveProperties()
        {
            var properties = db.Properties.Where(p => p.ApprovalStatus == "Not Approved").ToString();
            return View();
        }

        [SessionAuthorize("admin", "superadmin", "moderator")]
        public ActionResult Approve(int propertyId, string tableName)
        {
            string sql = $"SELECT * FROM {tableName} WHERE PropertyId = @propertyId";
            var propertyIdParam = new SqlParameter("propertyId", propertyId);
            var properties = db.Database.SqlQuery<PropertyDetailsTemplate>(sql, propertyIdParam).ToList();

            return View(properties);
        }

        [SessionAuthorize("admin", "superadmin", "moderator")]
        public ActionResult DeleteProperty(int propertyId, string tableName)
        {
            var property = db.Properties.FirstOrDefault(p => p.PropertyId == propertyId);
            if (property != null)
            {
                if (property.Deleted == true)
                {
                    TempData["AlreadyDeleted"] = true;
                }
                else
                {
                    property.Deleted = true;
                    db.Entry(property).State = EntityState.Modified;
                    db.SaveChanges();

                    if (!string.IsNullOrEmpty(tableName))
                    {
                        db.Database.ExecuteSqlCommand($"DROP TABLE {tableName}");
                    }
                }
            }

            return RedirectToAction("Properties", "Staff");
        }

        [SessionAuthorize("superadmin")]
        public ActionResult DeleteUser(int userId)
        {
            var user = db.Users.FirstOrDefault(p => p.UserId == userId);
            if (user != null)
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
                }
            }

            return RedirectToAction("Users", "Staff");
        }

        [SessionAuthorize("superadmin")]
        public ActionResult ChangeUserLevel(int userId, string userlevel)
        {
            var user = db.Staffs.FirstOrDefault(p => p.UserId == userId);

            if (user == null)
            {
                user = new Staff
                {
                    UserId = userId,
                    UserLevel = userlevel
                };

                db.Staffs.Add(user);
            }
            else
            {
                user.UserLevel = userlevel;
                db.Entry(user).State = EntityState.Modified;
            }

            if (userlevel == "None")
            {
                if (db.Entry(user).State == EntityState.Detached)
                {
                    db.Staffs.Attach(user);
                }
                db.Staffs.Remove(user);
            }


            db.SaveChanges();
            return RedirectToAction("Userlevel", "Staff");
        }

    }
}