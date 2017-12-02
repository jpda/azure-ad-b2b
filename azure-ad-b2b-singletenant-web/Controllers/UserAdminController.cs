using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace azure_ad_b2b_singletenant_web.Controllers
{
    public class UserAdminController : Controller
{
    // GET: /<controller>/
    public IActionResult Index()
    {
        return View();
    }
}
}
