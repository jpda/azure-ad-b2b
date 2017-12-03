using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using azure_ad_b2b_services;

namespace azure_ad_b2b_singletenant_web.Controllers
{
    [Authorize]
    public class AuthenticatedTenantController : Controller
    {
        public string MyIssuer
        {
            get { return User.Claims.SingleOrDefault(x => x.Type == "http://schemas.jpd.ms/aad/tenantId")?.Value; }
        }

        public IAppService _appService;

        public AuthenticatedTenantController(IAppService appService) : this()
        {
            _appService = appService;
        }

        public AuthenticatedTenantController() { }
    }
}