using CYHBrandRepo;
using CYHBrandUI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CYHBrandUI.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public HomeController(IHttpContextAccessor http, IConfiguration config,
                              IUnitOfWork uof, IUrlEncryptor enc)
            : base(http, config, uof, enc) { }

        public async Task<IActionResult> Index()
        {
            if (CurrentUser == null) return RedirectToAction("Login", "Account");
            var model = await UOF.DoctorRepo.GetByIdAsync(CurrentUser.UserId);
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult Error() => View();
    }
}
