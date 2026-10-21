using CYHBrandRepo;
using CYHBrandUI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CYHBrandUI.Controllers
{
    [Authorize]
    public class HelpController : BaseController
    {
        public HelpController(IHttpContextAccessor http, IConfiguration config, IUnitOfWork uof, IUrlEncryptor enc)
            : base(http, config, uof, enc) { }

        public IActionResult Index()
        {
            ViewData["Title"] = "Help & Support";
            ViewData["PageSubtitle"] = "Get help and find answers to your questions.";
            return View();
        }
    }
}
