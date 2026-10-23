using CYHBrandModel.DTO;
using CYHBrandRepo;
using CYHBrandUI.Helpers;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace CYHBrandUI.Areas.Admin.Controllers
{
    public class DoctorsController : AdminBaseController
    {
        public DoctorsController(IHttpContextAccessor http, IConfiguration config,
                                 IUnitOfWork uof, IUrlEncryptor enc)
            : base(http, config, uof, enc)
        {
        }

        public async Task<IActionResult> Index(string? searchKey, int pageNo = 1, int pageSize = 15)
        {
            var paged = await UOF.DoctorRepo.GetListAsync(searchKey, pageNo, pageSize);
            ViewBag.SearchKey = searchKey;
            return View(paged);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int doctorId, bool isActive)
        {
            var adminId = CurrentUser?.UserId ?? 0;
            var res = await UOF.DoctorRepo.ToggleStatusAsync(doctorId, isActive, adminId);
            return Json(new { success = res.IsSuccess, message = res.Message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCredits(int doctorId, int credits, string? reason)
        {
            if (credits <= 0)
                return Json(new { success = false, message = "Credits must be greater than 0." });

            var res = await UOF.DoctorRepo.AddAiCreditsAsync(
                doctorId, credits, reason ?? "Admin allocation from control panel");

            return Json(new { success = res, message = res ? $"{credits} credits added successfully." : "Failed to add credits." });
        }
    }
}
