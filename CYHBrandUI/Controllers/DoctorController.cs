using CYHBrandModel.DTO;
using CYHBrandRepo;
using CYHBrandUI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CYHBrandUI.Controllers
{
    [Authorize]
    public class DoctorController : BaseController
    {
        private readonly IWebHostEnvironment _env;

        public DoctorController(IHttpContextAccessor http, IConfiguration config,
                                IUnitOfWork uof, IUrlEncryptor enc, IWebHostEnvironment env)
            : base(http, config, uof, enc) { _env = env; }

        public async Task<IActionResult> Profile()
        {
            if (CurrentUser == null) return RedirectToAction("Login", "Account");
            var model = await UOF.DoctorRepo.GetByIdAsync(CurrentUser.UserId);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            if (CurrentUser == null) return RedirectToAction("Login", "Account");
            var model = await UOF.DoctorRepo.GetByIdAsync(CurrentUser.UserId);
            return View(model);
        }

        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(DoctorMasterDTO model)
        {
            if (CurrentUser == null) return RedirectToAction("Login", "Account");
            model.DoctorId = CurrentUser.UserId;

            // 1. Update Personal & Clinic profile details
            var result = await UOF.DoctorRepo.UpdateProfileAsync(model);

            // 2. Also update brand styling if provided
            if (!string.IsNullOrEmpty(model.PrimaryColorHex) || !string.IsNullOrEmpty(model.SecondaryColorHex) || !string.IsNullOrEmpty(model.PreferredFont))
            {
                await UOF.DoctorRepo.UpdateBrandSettingsAsync(model);
            }

            if (result.IsSuccess)
            {
                // 3. Refresh user session cookie so changes reflect everywhere immediately
                var updated = await UOF.DoctorRepo.GetByIdAsync(CurrentUser.UserId);
                if (updated != null)
                {
                    var principal = new CustomPrincipal
                    {
                        UserId = updated.DoctorId,
                        Name = updated.DisplayName ?? updated.FullName,
                        FullName = updated.FullName,
                        Email = updated.Email,
                        MobileNo = updated.MobileNo,
                        Specialty = updated.Specialty,
                        ClinicName = updated.ClinicName,
                        ProfilePic = updated.ProfilePic,
                        IsProfileComplete = updated.IsProfileComplete,
                        AiCredits = updated.AiCredits,
                        Roles = CurrentUser.Roles
                    };
                    await SignInUser(principal, isPersistent: true);
                }

                TempData["SuccessMsg"] = "Profile and clinic information saved successfully!";
                return RedirectToAction("EditProfile");
            }

            TempData["ErrorMsg"] = result.Message;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBrandSettings(DoctorMasterDTO model)
        {
            if (CurrentUser == null) return JsonFail("Session expired");
            model.DoctorId = CurrentUser.UserId;
            var result = await UOF.DoctorRepo.UpdateBrandSettingsAsync(model);
            return result.IsSuccess ? JsonOk("Brand settings saved.") : JsonFail(result.Message);
        }

        [HttpPost]
        public async Task<IActionResult> UploadProfilePic(IFormFile file)
        {
            if (CurrentUser == null) return JsonFail("Session expired");
            if (file == null || file.Length == 0) return JsonFail("Please select a file.");
            var allowed = new[] { ".jpg", ".jpeg", ".png" };
            if (!allowed.Contains(Path.GetExtension(file.FileName).ToLower()))
                return JsonFail("Only JPG/PNG allowed.");
            if (file.Length > 5 * 1024 * 1024) return JsonFail("Max 5 MB.");
            try
            {
                var folder   = Path.Combine(_env.WebRootPath, "uploads", "profile");
                Directory.CreateDirectory(folder);
                var ext      = Path.GetExtension(file.FileName).ToLower();
                var fileName = $"doc_{CurrentUser.UserId}_{DateTime.Now:yyyyMMddHHmmss}{ext}";
                await using var fs = new FileStream(Path.Combine(folder, fileName), FileMode.Create);
                await file.CopyToAsync(fs);
                var dbPath = $"/uploads/profile/{fileName}";
                var result = await UOF.DoctorRepo.UpdateProfilePicAsync(CurrentUser.UserId, dbPath);
                if (result.IsSuccess)
                {
                    var principal = CurrentUser;
                    principal.ProfilePic = dbPath;
                    await SignInUser(principal, isPersistent: true);
                    return Json(new { isSuccess = true, msg = "Doctor photo uploaded successfully.", filePath = dbPath });
                }
                return JsonFail(result.Message);
            }
            catch (Exception ex) { return JsonFail(ex.Message); }
        }

        [HttpPost]
        public async Task<IActionResult> UploadClinicLogo(IFormFile file)
        {
            if (CurrentUser == null) return JsonFail("Session expired");
            if (file == null || file.Length == 0) return JsonFail("Please select a file.");
            if (file.Length > 5 * 1024 * 1024) return JsonFail("Max 5 MB.");
            try
            {
                var folder   = Path.Combine(_env.WebRootPath, "uploads", "logos");
                Directory.CreateDirectory(folder);
                var ext      = Path.GetExtension(file.FileName).ToLower();
                var fileName = $"logo_{CurrentUser.UserId}_{DateTime.Now:yyyyMMddHHmmss}{ext}";
                await using var fs = new FileStream(Path.Combine(folder, fileName), FileMode.Create);
                await file.CopyToAsync(fs);
                var dbPath = $"/uploads/logos/{fileName}";
                var result = await UOF.DoctorRepo.UpdateClinicLogoAsync(CurrentUser.UserId, dbPath);
                return result.IsSuccess
                    ? Json(new { isSuccess = true, msg = "Logo uploaded.", filePath = dbPath })
                    : JsonFail(result.Message);
            }
            catch (Exception ex) { return JsonFail(ex.Message); }
        }
    }
}
