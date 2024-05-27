using System.Linq;
using System.Web.Mvc;
using VirtualVistaHub.Models;
using VirtualVistaHub.Filters;
using System;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace VirtualVistaHub.Controllers
{
    public class EditPropertyViewModel
    {
        public VirtualVistaHub.Models.Property Property { get; set; }
        public VirtualVistaHub.Models.PropertyDetailsTemplate PropertyDetails { get; set; }
        public string TableName { get; set; }
        public int UserId { get; set; }
        public string[] ImagePaths { get; set; }
        public EditPropertyViewModel() { }
    }

    public class PropertyController : Controller
    {
        readonly VirtualVistaBaseEntities db = new VirtualVistaBaseEntities();

        public static string GenerateTableName()
        {
            return "PropertyDetails_" + Guid.NewGuid().ToString("N");
        }

        [SessionAuthorize]
        public ActionResult Visual()
        {
            return View();
        }

        [SessionAuthorize]
        public ActionResult Information()
        {
            return View();
        }

        [SessionAuthorize]
        public ActionResult VisualDetails(int propertyId, string tableName)
        {
            string sql = $"SELECT * FROM {tableName} WHERE PropertyId = @propertyId";
            var propertyIdParam = new SqlParameter("propertyId", propertyId);
            var properties = db.Database.SqlQuery<PropertyDetailsTemplate>(sql, propertyIdParam).ToList();

            return View(properties);
        }

        [HttpGet]
        [SessionAuthorize]
        public ActionResult EditProperty(int propertyId, string tableName)
        {
            var information = db.Properties.FirstOrDefault(p => p.PropertyId == propertyId);

            if (information == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            if (Session["idUser"].ToString() == information.UserId.ToString() || Session["userLevel"].ToString() != "none")
            {
                string sql = $"SELECT * FROM {tableName} WHERE PropertyId = @propertyId";
                var propertyIdParam = new SqlParameter("propertyId", propertyId);
                var visual = db.Database.SqlQuery<PropertyDetailsTemplate>(sql, propertyIdParam).FirstOrDefault();

                string imageSql = $"SELECT Images FROM {tableName}";
                var images = db.Database.SqlQuery<string>(imageSql).ToArray();

                var model = new EditPropertyViewModel
                {
                    Property = information,
                    PropertyDetails = visual,
                    TableName = tableName,
                    UserId = visual.UserId,
                    ImagePaths = images
                };

                return View(model);
            }
            else
            {
                return RedirectToAction("Unauthorized", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProperty(EditPropertyViewModel model, IEnumerable<HttpPostedFileBase> newImages)
        {
            if (ModelState.IsValid)
            {
                var property = model.Property;
                property.PropertyDetailsTable = model.TableName;
                property.UserId = model.UserId;
                db.Entry(property).State = EntityState.Modified;
                db.SaveChanges();

                var propertyDetails = model.PropertyDetails;
                string tableName = model.TableName;
                string updatePropertyDetailsQuery = $"UPDATE {tableName} SET VTour = @VTour WHERE PropertyId = @PropertyId";
                SqlParameter[] propertyDetailsParams =
                {
            new SqlParameter("@VTour", propertyDetails.VTour),
            new SqlParameter("@PropertyId", property.PropertyId)
        };
                db.Database.ExecuteSqlCommand(updatePropertyDetailsQuery, propertyDetailsParams);

                if (newImages != null)
                {
                    string uploadDir = Server.MapPath($"~/Uploads/{tableName}");
                    var oldImages = Request.Form.GetValues("OldImages");

                    int index = 0;
                    foreach (var image in newImages)
                    {
                        if (image != null && image.ContentLength > 0)
                        {
                            string fileExtension = Path.GetExtension(image.FileName);
                            string randomFileName = $"{Guid.NewGuid()}{fileExtension}";
                            string filePath = Path.Combine(uploadDir, randomFileName);
                            image.SaveAs(filePath);

                            string oldImage = oldImages[index];
                            string updateSql = $@"
                            UPDATE {tableName} 
                            SET Images = @Images 
                            WHERE PropertyId = @PropertyId AND Images = @OldImage";

                            db.Database.ExecuteSqlCommand(
                                updateSql,
                                new SqlParameter("@PropertyId", property.PropertyId),
                                new SqlParameter("@Images", randomFileName),
                                new SqlParameter("@OldImage", oldImage)
                            );

                            DeleteImage(oldImage, property.PropertyId, model.TableName);
                        }
                        index++;
                    }
                }
            }

            if (Session["userLevel"].ToString() != "none")
                return RedirectToAction("Properties", "Staff");
            else
                return RedirectToAction("Properties", "Home");
        }


        [HttpPost]
        [SessionAuthorize]
        public ActionResult DeleteImage(string imageName, int propertyId, string tableName)
        {
            string uploadDir = Server.MapPath($"~/Uploads/{tableName}");
            string filePath = Path.Combine(uploadDir, imageName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);

                string deleteSql = $@"
        DELETE FROM {tableName}
        WHERE PropertyId = @PropertyId AND Images = @Images";

                db.Database.ExecuteSqlCommand(
                    deleteSql,
                    new SqlParameter("@PropertyId", propertyId),
                    new SqlParameter("@Images", imageName)
                );

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }



        [HttpPost]
        [SessionAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult Information(Property property)
        {
            var userId = Session["idUser"];

            if (ModelState.IsValid)
            {
                property.UserId = int.Parse(userId.ToString());
                property.PropertyDetailsTable = GenerateTableName().ToString();
                db.Properties.Add(property);

                db.SaveChanges();

                Session["idProperty"] = property.PropertyId.ToString();
                Session["tableDetails"] = property.PropertyDetailsTable.ToString();

                string tableName = property.PropertyDetailsTable;

                string createTableSql = $@"
                CREATE TABLE {tableName} (
	                [PropertyId] INT FOREIGN KEY REFERENCES Property([PropertyId]),
	                [VTour] NVARCHAR(MAX) NOT NULL,
	                [Images] NVARCHAR(MAX) NOT NULL,
	                [UserId] INT FOREIGN KEY REFERENCES Users([UserId]) NOT NULL
                );";

                db.Database.ExecuteSqlCommand(createTableSql);

            }

            return RedirectToAction("Visual", "Property");
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult Visual(PropertyDetailsTemplate property, IEnumerable<HttpPostedFileBase> images)
        {
            var userId = Session["idUser"];
            var propertyId = Session["idProperty"];
            var tableName = Session["tableDetails"];

            string uploadDir = Server.MapPath($"~/Uploads/{tableName}");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            foreach (var image in images)
            {
                if (image != null && image.ContentLength > 0)
                {
                    string fileExtension = Path.GetExtension(image.FileName);
                    string randomFileName = $"{Guid.NewGuid()}{fileExtension}";
                    string filePath = Path.Combine(uploadDir, randomFileName);
                    image.SaveAs(filePath);

                    string insertSql = $@"
                    INSERT INTO {tableName} (PropertyId, VTour, Images, UserId)
                    VALUES (@PropertyId, @VTour, @Images, @UserId);";

                    db.Database.ExecuteSqlCommand(
                        insertSql,
                        new SqlParameter("@PropertyId", int.Parse(propertyId.ToString())),
                        new SqlParameter("@VTour", property.VTour),
                        new SqlParameter("@Images", randomFileName),
                        new SqlParameter("@UserId", int.Parse(userId.ToString()))
                    );
                }
            }

            return RedirectToAction("Properties", "Home");
        }

    }
}
