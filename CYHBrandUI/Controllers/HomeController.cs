using CYHBrandModel.DTO;
using CYHBrandRepo;
using CYHBrandUI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CYHBrandUI.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IMemoryCache _cache;

        public HomeController(IHttpContextAccessor http, IConfiguration config,
                              IUnitOfWork uof, IUrlEncryptor enc, IMemoryCache cache)
            : base(http, config, uof, enc)
        {
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            if (CurrentUser == null) return RedirectToAction("Login", "Account");

            var userId = CurrentUser.UserId;
            var cacheKey = $"dashboard_{userId}";

            var dashboard = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);
                var doctorDto     = await UOF.DoctorRepo.GetByIdAsync(userId);
                var recentDesigns = await UOF.DesignRepo.GetRecentByDoctorAsync(userId, 6);
                var totalDesigns  = await UOF.DesignRepo.CountByDoctorAsync(userId);

                return new DashboardDTO
                {
                    TotalDesigns   = totalDesigns,
                    ScheduledPosts = 0,
                    TotalShares    = 0,
                    SocialReach    = 0,
                    RecentDesigns  = recentDesigns,
                    TodaysDesigns  = recentDesigns
                        .Where(d => d.CreatedAt.Date == DateTime.Today).ToList()
                };
            }) ?? new DashboardDTO();

            var doctorInfo = await UOF.DoctorRepo.GetByIdAsync(userId);
            ViewBag.DoctorName = doctorInfo?.FullName ?? doctorInfo?.DisplayName ?? CurrentUser.Name;
            return View(dashboard);
        }

        [AllowAnonymous]
        public IActionResult Error() => View();
    }
}
