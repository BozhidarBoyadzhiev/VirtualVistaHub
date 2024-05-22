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

        [SessionAuthorize("admin", "superadmin")]
        public ActionResult Users()
        {
            var users = db.Users.Include(p => p.Staff).ToList();

            return View(users);
        }

        [SessionAuthorize("admin", "superadmin")]
        public ActionResult UserLevel()
        {
            var users = db.Users.Include(p => p.Staff).ToList();
            return View(users);
        }

        [SessionAuthorize("admin", "superadmin", "moderator")]
        public ActionResult ApproveInformation()
        {
            var properties = db.Properties.Where(p => p.ApprovalStatus == "Not Approved").ToList();
            return View(properties);
        }

        [SessionAuthorize("admin", "superadmin", "moderator")]
        public ActionResult ApproveVisuals(int propertyId, string tableName)
        {
            string sql = $"SELECT * FROM {tableName} WHERE PropertyId = @propertyId";
            var propertyIdParam = new SqlParameter("propertyId", propertyId);
            var properties = db.Database.SqlQuery<PropertyDetailsTemplate>(sql, propertyIdParam).ToList();

            return View(properties);
        }

        [HttpGet]
        [SessionAuthorize("admin", "superadmin", "moderator")]
        public ActionResult Approve(int propertyId, bool approvedstatus = false, string approvedmessage = "")
        {
            var property = db.Properties.FirstOrDefault(p => p.PropertyId == propertyId);

            if (property == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            if (approvedstatus == true)
                property.ApprovalStatus = "Approved";
            else
                property.ApprovalStatus = approvedmessage;

            db.Entry(property).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("ApproveInformation", "Staff");
        }        

        [HttpGet]
        [SessionAuthorize("admin", "superadmin", "moderator")]
        public ActionResult DeleteProperty(int propertyId, string tableName)
        {
            var property = db.Properties.FirstOrDefault(p => p.PropertyId == propertyId);

            if (property == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

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

        [HttpPost]
        [SessionAuthorize("superadmin")]
        public ActionResult ChangeUserLevel(int userId, string userlevel)
        {
            var user = db.Staffs.FirstOrDefault(p => p.UserId == userId);

            if (user == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

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