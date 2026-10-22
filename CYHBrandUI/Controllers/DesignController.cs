using CYHBrandModel.DTO;
using CYHBrandRepo;
using CYHBrandUI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CYHBrandUI.Controllers
{
    [Authorize]
    public class DesignController : BaseController
    {
        private readonly IWebHostEnvironment _env;

        public DesignController(IHttpContextAccessor http, IConfiguration config,
                                IUnitOfWork uof, IUrlEncryptor enc, IWebHostEnvironment env)
            : base(http, config, uof, enc) { _env = env; }

        // GET /Design
        public async Task<IActionResult> Index(string? status, string? searchKey, int pageNo = 1)
        {
            if (CurrentUser == null) return RedirectToAction("Login", "Account");
            var paged = await UOF.DesignRepo.GetListAsync(CurrentUser.UserId, status, searchKey, pageNo, 12);
            // Encrypt IDs before sending to view
            foreach (var d in paged.Items)
                d.EncId = Enc.Encrypt(d.DesignId);

            var vm = new BannerDesignDTO
            {
                DesignList   = paged.Items,
                FilterStatus = status,
                SearchKey    = searchKey,
                PageNo       = pageNo,
                TotalPages   = paged.TotalPages,
                TotalRecords = paged.TotalRecord
            };
            return View(vm);
        }

        // GET /Design/Detail/{encId}
        public async Task<IActionResult> Detail(string id)
        {
            if (CurrentUser == null) return RedirectToAction("Login", "Account");
            var realId = Enc.Decrypt(id);
            if (realId == null)
            {
                TempData["ErrorMsg"] = "Design not found or invalid link.";
                return RedirectToAction("Index");
            }
            var model = await UOF.DesignRepo.GetByIdAsync(realId.Value, CurrentUser.UserId);
            if (model == null)
            {
                TempData["ErrorMsg"] = "Design not found.";
                return RedirectToAction("Index");
            }
            model.EncId = Enc.Encrypt(model.DesignId);
            return View(model);
        }

        // GET /Design/Create?templateId={encId}
        [HttpGet]
        public async Task<IActionResult> Create(string? templateId)
        {
            if (CurrentUser == null) return RedirectToAction("Login", "Account");
            var model = new BannerDesignDTO { DoctorId = CurrentUser.UserId };
            if (!string.IsNullOrEmpty(templateId))
            {
                var realTmplId = Enc.Decrypt(templateId);
                if (realTmplId.HasValue)
                {
                    var t = await UOF.TemplateRepo.GetByIdAsync(realTmplId.Value);
                    if (t != null)
                    {
                        model.SourceTemplateId = t.TemplateId;
                        model.DesignJson       = t.TemplateJson;
                        model.Orientation      = t.Orientation;
                        model.WidthPx          = t.WidthPx;
                        model.HeightPx         = t.HeightPx;
                        ViewBag.Template = t;
                    }
                }
            }
            return View(model);
        }

        // GET /Design/Edit/{encId}  — load existing design into editor
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (CurrentUser == null) return RedirectToAction("Login", "Account");
            var realId = Enc.Decrypt(id);
            if (realId == null)
            {
                TempData["ErrorMsg"] = "Design not found or link has expired.";
                return RedirectToAction("Index");
            }
            var model = await UOF.DesignRepo.GetByIdAsync(realId.Value, CurrentUser.UserId);
            if (model == null)
            {
                TempData["ErrorMsg"] = "Design not found.";
                return RedirectToAction("Index");
            }
            model.EncId = Enc.Encrypt(model.DesignId);
            return View("Create", model);   // reuse Create view
        }

        // POST /Design/Save  (AJAX JSON body)
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] BannerDesignDTO model)
        {
            if (CurrentUser == null) return JsonFail("Session expired");
            model.DoctorId = CurrentUser.UserId;

            // Decrypt EncId → real DesignId
            if (!string.IsNullOrEmpty(model.EncId))
            {
                var realId = Enc.Decrypt(model.EncId);
                model.DesignId = realId ?? (model.DesignId > 0 ? model.DesignId : 0);
            }

            // Save thumbnail base64 → disk
            if (!string.IsNullOrEmpty(model.ThumbnailPath) &&
                model.ThumbnailPath.StartsWith("data:image"))
            {
                try
                {
                    var folder   = Path.Combine(_env.WebRootPath, "uploads", "thumbnails");
                    Directory.CreateDirectory(folder);
                    var comma    = model.ThumbnailPath.IndexOf(',');
                    var header   = model.ThumbnailPath[..comma];
                    var base64   = model.ThumbnailPath[(comma + 1)..];
                    var ext      = header.Contains("png") ? "png" : "jpg";
                    var fileName = $"thumb_{CurrentUser.UserId}_{DateTime.Now:yyyyMMddHHmmss}.{ext}";
                    await System.IO.File.WriteAllBytesAsync(
                        Path.Combine(folder, fileName),
                        Convert.FromBase64String(base64));
                    model.ThumbnailPath = $"/uploads/thumbnails/{fileName}";
                }
                catch { model.ThumbnailPath = null; }
            }

            var result = await UOF.DesignRepo.SaveAsync(model);
            if (result.IsSuccess)
            {
                var encId = result.Id.HasValue ? Enc.Encrypt(result.Id.Value) : "";
                return Json(new { isSuccess = true, msg = result.Message, id = result.Id, encId });
            }
            return JsonFail(result.Message);
        }

        // POST /Design/Delete  (AJAX)
        [HttpPost]
        public async Task<IActionResult> Delete(string? id)
        {
            if (CurrentUser == null) return JsonFail("Session expired");
            if (string.IsNullOrEmpty(id) && Request.HasFormContentType && Request.Form.ContainsKey("id"))
            {
                id = Request.Form["id"].ToString();
            }
            if (string.IsNullOrEmpty(id)) return JsonFail("Invalid ID");
            var realId = Enc.Decrypt(id);
            if (realId == null) return JsonFail("Invalid ID");
            var result = await UOF.DesignRepo.DeleteAsync(realId.Value, CurrentUser.UserId);
            return result.IsSuccess ? JsonOk(result.Message) : JsonFail(result.Message);
        }
    }
}
