using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using azure_ad_b2b_services;
using azure_ad_b2b_services.AppTenantRepo;

namespace azure_ad_b2b_singletenant_web.Controllers
{
    public class TenantAdminController : Controller
    {
        private IAppService _appService;
        public TenantAdminController(IAppService appService)
        {
            _appService = appService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public async Task<IActionResult> List()
        {
            var model = await _appService.GetAllTenantsAsync();
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(AppTenant tenant)
        {
            tenant.DateAdded = DateTime.UtcNow;
            tenant.InvitedBy = HttpContext.User.Identity.Name;
            tenant.InviteSent = false;
            var t = await _appService.AddTenantAsync(tenant);
            return View();
        }
    }
}