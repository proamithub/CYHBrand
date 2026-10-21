using CYHBrandRepo;
using CYHBrandUI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CYHBrandUI.Controllers
{
    [Authorize]
    public class AiBannerController : BaseController
    {
        public AiBannerController(IHttpContextAccessor http, IConfiguration config, IUnitOfWork uof, IUrlEncryptor enc)
            : base(http, config, uof, enc) { }

        public IActionResult Index()
        {
            ViewData["Title"] = "AI Banner Creator";
            ViewData["PageSubtitle"] = "Create stunning health-awareness banners with AI in seconds.";
            return View();
        }
    }
}
