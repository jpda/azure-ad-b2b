using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using azure_ad_b2b_services;
using Microsoft.AspNetCore.Authorization;

namespace azure_ad_b2b_singletenant_web.Controllers
{
    [Authorize(Roles="CustomerAdmin")]
    public class UserAdminController : Controller
    {
        private IAppService _appService;
        public UserAdminController(IAppService appService)
        {
            _appService = appService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public async Task<IActionResult> List()
        {
            //var userTenant = 
            var model = await _appService.GetAllUsersByTenantAsync("");
            return View(model);
        }
    }
}