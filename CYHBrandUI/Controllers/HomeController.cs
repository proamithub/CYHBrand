using CYHBrandModel.DTO;
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

            // Build dashboard summary
            var doctorDto = await UOF.DoctorRepo.GetByIdAsync(CurrentUser.UserId);
            var recentDesigns = await UOF.DesignRepo.GetRecentByDoctorAsync(CurrentUser.UserId, 6);

            var dashboard = new DashboardDTO
            {
                TotalDesigns  = await UOF.DesignRepo.CountByDoctorAsync(CurrentUser.UserId),
                ScheduledPosts = 0,   // Phase 8 — calendar
                TotalShares   = 0,   // Phase 9 — publish
                SocialReach   = 0,   // Phase 9 — analytics
                RecentDesigns = recentDesigns,
                TodaysDesigns = recentDesigns
                    .Where(d => d.CreatedAt.Date == DateTime.Today)
                    .ToList(),
            };

            ViewBag.DoctorName = doctorDto?.FullName ?? doctorDto?.DisplayName ?? CurrentUser.Name;
            return View(dashboard);
        }

        [AllowAnonymous]
        public IActionResult Error() => View();
    }
}
