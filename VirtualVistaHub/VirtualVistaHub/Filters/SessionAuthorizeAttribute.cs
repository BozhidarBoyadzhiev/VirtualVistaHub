using System;
using System.Web;
using System.Web.Mvc;

namespace VirtualVistaHub.Filters
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["idUser"] == null)
            {
                filterContext.Result = new RedirectResult("~/Home/Login");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
