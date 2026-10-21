using CYHBrandRepo;
using CYHBrandUI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CYHBrandUI.Controllers
{
    [Authorize]
    public class ShareController : BaseController
    {
        public ShareController(IHttpContextAccessor http, IConfiguration config, IUnitOfWork uof, IUrlEncryptor enc)
            : base(http, config, uof, enc) { }

        public IActionResult Index()
        {
            ViewData["Title"] = "Share & Publish";
            ViewData["PageSubtitle"] = "Share your design on social media or download for later.";
            return View();
        }
    }
}
