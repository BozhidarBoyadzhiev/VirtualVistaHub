using System.Linq;
using System.Web.Mvc;
using VirtualVistaHub.Models;
using VirtualVistaHub.Filters;

namespace VirtualVistaHub.Controllers
{
    public class PropertyController : Controller
    {
        VirtualVistaBaseEntities db = new VirtualVistaBaseEntities();

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

        [HttpPost]
        [SessionAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult Information(Property property)
        {
            var userId = Session["idUser"];
            if (userId == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (ModelState.IsValid)
            {
                property.UserId = int.Parse(userId.ToString());
                db.Properties.Add(property);
                db.SaveChanges();

                Session["idProperty"] = property.PropertyId.ToString(); // Assuming you want PropertyId, not UserId

                return RedirectToAction("Properties", "Home");
            }

            return View(property);
        }
    }
}
