using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VirtualVistaHub.Models;

namespace VirtualVistaHub.Filters
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string[] allowedUserLevels;

        public SessionAuthorizeAttribute(params string[] userLevels)
        {
            this.allowedUserLevels = userLevels ?? new string[] { };
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["idUser"] == null)
            {
                filterContext.Result = new RedirectResult("~/Home/Login");
                return;
            }

            if (allowedUserLevels.Length > 0)
            {
                var userLevel = HttpContext.Current.Session["userLevel"];
                using (var db = new VirtualVistaBaseEntities())
                {
                    var user = db.Staffs.FirstOrDefault(u => u.UserLevel == userLevel.ToString());
                    if (user == null || !allowedUserLevels.Contains(user.UserLevel))
                    {
                        filterContext.Result = new RedirectResult("~/Home/Unauthorized");
                        return;
                    }
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
