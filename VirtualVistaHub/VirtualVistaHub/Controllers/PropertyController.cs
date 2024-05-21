using System.Linq;
using System.Web.Mvc;
using VirtualVistaHub.Models;
using VirtualVistaHub.Filters;
using System;
using System.Data.SqlClient;
using System.Data;

namespace VirtualVistaHub.Controllers
{
    public class PropertyController : Controller
    {
        readonly VirtualVistaBaseEntities db = new VirtualVistaBaseEntities();

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


        public static string GenerateTableName()
        {
            return "PropertyDetails_" + Guid.NewGuid().ToString("N");
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

            // Insert data into the dynamically created table
            string insertSql = $@"
            INSERT INTO {tableName} (PropertyId, CoordinatesOfVTour, Image, Video, UserId)
            VALUES (@PropertyId, @CoordinatesOfVTour, @Image, @Video, @UserId);";

            // Execute the command with parameters
            db.Database.ExecuteSqlCommand(
                insertSql,
                new SqlParameter("@PropertyId", int.Parse(propertyId.ToString())),
                new SqlParameter("@CoordinatesOfVTour", property.CoordinatesOfVTour),
                new SqlParameter("@Image", property.Image),
                new SqlParameter("@Video", property.Video),
                new SqlParameter("@UserId", int.Parse(userId.ToString()))
            );

            return RedirectToAction("Properties", "Home");
        }

    }
}
