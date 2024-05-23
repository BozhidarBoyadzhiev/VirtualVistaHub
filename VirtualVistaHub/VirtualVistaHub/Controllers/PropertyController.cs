using System.Linq;
using System.Web.Mvc;
using VirtualVistaHub.Models;
using VirtualVistaHub.Filters;
using System;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity;

namespace VirtualVistaHub.Controllers
{
    public class EditPropertyViewModel
    {
        public VirtualVistaHub.Models.Property Property { get; set; }
        public VirtualVistaHub.Models.PropertyDetailsTemplate PropertyDetails { get; set; }
        public string TableName { get; set; }
        public int UserId { get; set; }
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

                var model = new EditPropertyViewModel
                {
                    Property = information,
                    PropertyDetails = visual,
                    TableName = tableName,
                    UserId = visual.UserId
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
        public ActionResult EditProperty(EditPropertyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var property = model.Property;

                property.PropertyDetailsTable = model.TableName;
                property.UserId = model.UserId;
                db.Entry(property).State = EntityState.Modified;
                db.SaveChanges();

                var propertyDetails = model.PropertyDetails;

                string updatePropertyDetailsQuery = $"UPDATE {model.TableName} SET CoordinatesOfVTour = @CoordinatesOfVTour, Images = @Images WHERE PropertyId = @PropertyId";
                SqlParameter[] propertyDetailsParams =
                {
                    new SqlParameter("@CoordinatesOfVTour", propertyDetails.CoordinatesOfVTour),
                    new SqlParameter("@Images", propertyDetails.Images),
                    new SqlParameter("@PropertyId", property.PropertyId)
                };
                db.Database.ExecuteSqlCommand(updatePropertyDetailsQuery, propertyDetailsParams);

            }

            if (Session["userLevel"].ToString() != "none")
                return RedirectToAction("Properties", "Staff");
            else
                return RedirectToAction("Properties", "Home");
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
                    [CoordinatesOfVTour] NVARCHAR(MAX) NOT NULL,
                    [Image] NVARCHAR(MAX) NOT NULL,
                    [Video] NVARCHAR(MAX) NOT NULL,
                    [UserId] INT FOREIGN KEY REFERENCES Users([UserId]) NOT NULL,
                    PRIMARY KEY([PropertyId])
                );";

                db.Database.ExecuteSqlCommand(createTableSql);

            }

            return RedirectToAction("Visual", "Property");
        }

        [HttpPost]
        [SessionAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult Visual(PropertyDetailsTemplate property)
        {
            var userId = Session["idUser"];
            var propertyId = Session["idProperty"];

            var tableName = Session["tableDetails"];

            string insertSql = $@"
            INSERT INTO {tableName} (PropertyId, CoordinatesOfVTour, Images, UserId)
            VALUES (@PropertyId, @CoordinatesOfVTour, @Images, @UserId);";

            db.Database.ExecuteSqlCommand(
                insertSql,
                new SqlParameter("@PropertyId", int.Parse(propertyId.ToString())),
                new SqlParameter("@CoordinatesOfVTour", property.CoordinatesOfVTour),
                new SqlParameter("@Images", property.Images),
                new SqlParameter("@UserId", int.Parse(userId.ToString()))
            );

            return RedirectToAction("Properties", "Home");
        }

    }
}
