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
        public ActionResult Properties(int page = 1, int pageSize = 10)
        {
            var properties = db.Properties.Include(p => p.User);

            var totalItems = properties.Count();
            var propertyList = properties.OrderBy(p => p.PropertyId)
                                         .Skip((page - 1) * pageSize)
                                         .Take(pageSize)
                                         .ToList();

            var pagination = new PaginationModel
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            var model = new Tuple<IEnumerable<Property>, PaginationModel>(propertyList, pagination);

            return View(model);
        }


        [SessionAuthorize("admin", "superadmin")]
        public ActionResult Users(int page = 1, int pageSize = 10)
        {
            var users = db.Users.Include(u => u.Staff);

            var totalItems = users.Count();
            var userList = users.OrderBy(u => u.UserId)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            var pagination = new PaginationModel
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            var model = new Tuple<IEnumerable<User>, PaginationModel>(userList, pagination);

            return View(model);
        }

        [SessionAuthorize("admin", "superadmin")]
        public ActionResult UserLevel(int page = 1, int pageSize = 10)
        {
            var users = db.Users.Include(u => u.Staff);

            var totalItems = users.Count();
            var userList = users.OrderBy(u => u.UserId)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            var pagination = new PaginationModel
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            var model = new Tuple<IEnumerable<User>, PaginationModel>(userList, pagination);

            return View(model);
        }


        [SessionAuthorize("admin", "superadmin", "moderator")]
        public ActionResult ApproveInformation(int page = 1, int pageSize = 10)
        {
            var properties = db.Properties
                                .Where(p => p.ApprovalStatus == "Not Approved" && p.Deleted != true);

            var totalItems = properties.Count();
            var propertyList = properties.OrderBy(p => p.PropertyId)
                                         .Skip((page - 1) * pageSize)
                                         .Take(pageSize)
                                         .ToList();

            var pagination = new PaginationModel
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            var model = new Tuple<IEnumerable<Property>, PaginationModel>(propertyList, pagination);

            return View(model);
        }


        [SessionAuthorize("admin", "superadmin", "moderator")]
        public ActionResult ApproveVisuals(int propertyId, string tableName)
        {
            string sql = $"SELECT * FROM {tableName} WHERE PropertyId = @propertyId";
            var propertyIdParam = new SqlParameter("propertyId", propertyId);
            var properties = db.Database.SqlQuery<PropertyDetailsTemplate>(sql, propertyIdParam).FirstOrDefault();

            string imageSql = $"SELECT Images FROM {tableName}";
            var images = db.Database.SqlQuery<string>(imageSql).ToArray();

            var model = new ImagesModel
            {
                PropertyDetails = properties,
                TableName = tableName,
                ImagePaths = images
            };

            return View(model);
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

        [HttpGet]
        [SessionAuthorize("superadmin", "admin")]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize("superadmin", "admin")]
        public ActionResult EditUser(User user)
        {
            var existingUser = db.Users.Where(u => u.UserId == user.UserId).FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(user.Password) && user.Password != existingUser.Password)
            {
                existingUser.Password = HomeController.HashPassword(user.Password);
            }

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.Deleted = user.Deleted;

            db.Entry(existingUser).State = EntityState.Modified;
            db.SaveChanges();
           
            return RedirectToAction("Users", "Staff");
        }
    }
}