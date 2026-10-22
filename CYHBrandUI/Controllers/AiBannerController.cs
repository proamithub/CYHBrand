using CYHBrandModel.DTO;
using CYHBrandRepo;
using CYHBrandUI.Helpers;
using CYHBrandUI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CYHBrandUI.Controllers
{
    [Authorize]
    public class AiBannerController : BaseController
    {
        private readonly IAiBannerService _aiService;
        private readonly IWebHostEnvironment _env;

        public AiBannerController(
            IHttpContextAccessor http,
            IConfiguration config,
            IUnitOfWork uof,
            IUrlEncryptor enc,
            IAiBannerService aiService,
            IWebHostEnvironment env)
            : base(http, config, uof, enc)
        {
            _aiService = aiService;
            _env = env;
        }

        // GET /AiBanner
        [HttpGet]
        public async Task<IActionResult> Index(string? topic = null, string? lang = null)
        {
            ViewData["Title"] = "AI Banner Creator";
            ViewData["PageSubtitle"] = "Create professional, branded health-awareness banners with AI in seconds.";
            ViewBag.PreloadTopic = topic;
            ViewBag.PreloadLang = lang;

            int credits = 10;
            if (CurrentUser != null)
            {
                var doc = await UOF.DoctorRepo.GetByIdAsync(CurrentUser.UserId);
                if (doc != null)
                {
                    ViewBag.DoctorDto = doc;
                    credits = doc.AiCredits;
                }
                ViewBag.Doctor = CurrentUser;
            }
            ViewBag.AiCredits = credits;
            return View();
        }

        // GET /AiBanner/GetCredits
        [HttpGet]
        public async Task<IActionResult> GetCredits()
        {
            if (CurrentUser == null) return JsonFail("Session expired. Please log in again.");
            int credits = await UOF.DoctorRepo.GetAiCreditsAsync(CurrentUser.UserId);
            return Json(new { isSuccess = true, credits });
        }

        // POST /AiBanner/RechargeCredits
        [HttpPost]
        public async Task<IActionResult> RechargeCredits([FromBody] RechargeCreditsRequestDTO? req)
        {
            if (CurrentUser == null) return JsonFail("Session expired. Please log in again.");
            int add = (req?.Amount > 0) ? req.Amount : 10;
            bool ok = await UOF.DoctorRepo.AddAiCreditsAsync(CurrentUser.UserId, add, "User recharged AI credits");
            int credits = await UOF.DoctorRepo.GetAiCreditsAsync(CurrentUser.UserId);
            return Json(new { isSuccess = ok, credits, msg = $"Successfully added {add} AI Credits!" });
        }

        public class RechargeCreditsRequestDTO
        {
            public int Amount { get; set; } = 10;
        }

        // POST /AiBanner/Generate
        [HttpPost]
        public async Task<IActionResult> Generate([FromBody] AiBannerRequestDTO request)
        {
            if (CurrentUser == null) return JsonFail("Session expired. Please log in again.");
            if (string.IsNullOrWhiteSpace(request?.Topic))
                return JsonFail("Please enter a medical topic or select a suggestion chip.");

            // Always fetch the freshest doctor profile from DB so ProfilePic, Name, Specialty, Clinic are 100% current
            var doc = await UOF.DoctorRepo.GetByIdAsync(CurrentUser.UserId);
            var principal = CurrentUser;
            if (doc != null)
            {
                principal.ProfilePic = doc.ProfilePic;
                principal.FullName = !string.IsNullOrWhiteSpace(doc.FullName) ? doc.FullName : principal.FullName;
                principal.Specialty = !string.IsNullOrWhiteSpace(doc.Specialty) ? doc.Specialty : principal.Specialty;
                principal.ClinicName = !string.IsNullOrWhiteSpace(doc.ClinicName) ? doc.ClinicName : principal.ClinicName;
                principal.MobileNo = !string.IsNullOrWhiteSpace(doc.MobileNo) ? doc.MobileNo : principal.MobileNo;
            }

            var result = await _aiService.GenerateBannerAsync(request, principal);
            if (!result.IsSuccess)
                return JsonFail(result.Message);

            return Json(new { isSuccess = true, data = result });
        }

        // POST /AiBanner/SaveDesign
        [HttpPost]
        public async Task<IActionResult> SaveDesign([FromBody] BannerDesignDTO model)
        {
            if (CurrentUser == null) return JsonFail("Session expired. Please log in again.");
            model.DoctorId = CurrentUser.UserId;
            model.Status ??= "Saved";

            if (string.IsNullOrWhiteSpace(model.DesignJson))
            {
                return JsonFail("Design data is missing. Please regenerate the banner.");
            }

            // Save thumbnail if base64
            if (!string.IsNullOrEmpty(model.ThumbnailPath) && model.ThumbnailPath.StartsWith("data:image"))
            {
                try
                {
                    var folder = Path.Combine(_env.WebRootPath, "uploads", "thumbnails");
                    Directory.CreateDirectory(folder);
                    var comma = model.ThumbnailPath.IndexOf(',');
                    var header = model.ThumbnailPath[..comma];
                    var base64 = model.ThumbnailPath[(comma + 1)..];
                    var ext = header.Contains("png") ? "png" : "jpg";
                    var fileName = $"thumb_{CurrentUser.UserId}_{DateTime.Now:yyyyMMddHHmmss}.{ext}";
                    await System.IO.File.WriteAllBytesAsync(
                        Path.Combine(folder, fileName),
                        Convert.FromBase64String(base64));
                    model.ThumbnailPath = $"/uploads/thumbnails/{fileName}";
                }
                catch
                {
                    model.ThumbnailPath = null;
                }
            }

            var result = await UOF.DesignRepo.SaveAsync(model);
            if (result.IsSuccess && result.Id.HasValue)
            {
                string encId = Enc.Encrypt(result.Id.Value);
                return Json(new { isSuccess = true, msg = "Design saved to your library!", id = result.Id.Value, encId });
            }

            return JsonFail(result.Message ?? "Failed to save design");
        }
    }
}
