using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using azure_ad_b2b_singletenant_web.Models;

namespace azure_ad_b2b_singletenant_web.Controllers
{
    [Authorize]
    public class HomeController : AuthenticatedTenantController
    {
        public IActionResult Index()
        {
            ViewBag.Claims = HttpContext.User.Claims;
            ViewBag.Issuer = MyIssuer;
            return View();
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}