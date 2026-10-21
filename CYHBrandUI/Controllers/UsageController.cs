using CYHBrandRepo;
using CYHBrandUI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CYHBrandUI.Controllers
{
    [Authorize]
    public class UsageController : BaseController
    {
        public UsageController(IHttpContextAccessor http, IConfiguration config, IUnitOfWork uof, IUrlEncryptor enc)
            : base(http, config, uof, enc) { }

        public IActionResult Index()
        {
            ViewData["Title"] = "Usage & Subscription";
            ViewData["PageSubtitle"] = "View your plan usage and subscription details.";
            return View();
        }
    }
}
