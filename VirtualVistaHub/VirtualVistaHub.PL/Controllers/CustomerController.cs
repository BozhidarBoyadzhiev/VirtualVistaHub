using Microsoft.AspNetCore.Mvc;
using VirtualVistaHub.BLL;

namespace VirtualVistaHub.PL.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
