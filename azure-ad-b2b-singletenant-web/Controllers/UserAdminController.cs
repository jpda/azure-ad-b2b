using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using azure_ad_b2b_services;
using Microsoft.AspNetCore.Authorization;
using azure_ad_b2b_services.AppTenantRepo;
using System;

namespace azure_ad_b2b_singletenant_web.Controllers
{
    [Authorize(Roles = "CustomerAdmin")]
    public class UserAdminController : AuthenticatedTenantController
    {
        public UserAdminController(IAppService appService) : base(appService) { }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public async Task<IActionResult> List()
        {
            var model = await _appService.GetAllUsersByTenantAsync(MyIssuer);
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Issuer = MyIssuer;
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(AppUser u)
        {
            u.AddedBy = User.Identity.Name;
            u.TenantId = MyIssuer;
            u.DateAdded = DateTime.UtcNow;
            var user = await _appService.AddUserAsync(u, true, false, true);
            return RedirectToAction("Index");
        }
    }
}