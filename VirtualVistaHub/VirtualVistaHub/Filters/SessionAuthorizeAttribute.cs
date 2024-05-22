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

            var userLevel = HttpContext.Current.Session["userLevel"]?.ToString();

            if (allowedUserLevels.Length > 0 && userLevel != null)
            {
               if (userLevel == "none" && allowedUserLevels.Contains("none"))
                {
                    base.OnActionExecuting(filterContext);
                    return;
                }

                using (var db = new VirtualVistaBaseEntities())
                {
                    var user = db.Staffs.FirstOrDefault(u => u.UserLevel == userLevel);
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
