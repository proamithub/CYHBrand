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
        public IActionResult Index()
        {
            ViewData["Title"] = "AI Banner Creator";
            ViewData["PageSubtitle"] = "Create professional, branded health-awareness banners with AI in seconds.";
            ViewBag.Doctor = CurrentUser;
            return View();
        }

        // POST /AiBanner/Generate
        [HttpPost]
        public async Task<IActionResult> Generate([FromBody] AiBannerRequestDTO request)
        {
            if (CurrentUser == null) return JsonFail("Session expired. Please log in again.");
            if (string.IsNullOrWhiteSpace(request?.Topic))
                return JsonFail("Please enter a medical topic or select a suggestion chip.");

            var result = await _aiService.GenerateBannerAsync(request, CurrentUser);
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
