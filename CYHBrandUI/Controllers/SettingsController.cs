using CYHBrandRepo;
using CYHBrandUI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CYHBrandUI.Controllers
{
    [Authorize]
    public class SettingsController : BaseController
    {
        public SettingsController(IHttpContextAccessor http, IConfiguration config, IUnitOfWork uof, IUrlEncryptor enc)
            : base(http, config, uof, enc) { }

        public IActionResult Index()
        {
            ViewData["Title"] = "Settings";
            ViewData["PageSubtitle"] = "Configure your account and application preferences.";
            return View();
        }
    }
}
