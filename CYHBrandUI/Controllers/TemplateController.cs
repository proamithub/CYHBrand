using CYHBrandModel.DTO;
using CYHBrandRepo;
using CYHBrandUI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CYHBrandUI.Controllers
{
    [Authorize]
    public class TemplateController : BaseController
    {
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan _catCacheTtl  = TimeSpan.FromMinutes(30);
        private static readonly TimeSpan _listCacheTtl = TimeSpan.FromMinutes(5);

        public TemplateController(IHttpContextAccessor http, IConfiguration config,
                                  IUnitOfWork uof, IUrlEncryptor enc, IMemoryCache cache)
            : base(http, config, uof, enc)
        {
            _cache = cache;
        }

        public async Task<IActionResult> Index(string? categoryId, string? orientation,
                                               string? searchKey, int pageNo = 1)
        {
            int? realCatId = null;
            if (!string.IsNullOrEmpty(categoryId)) realCatId = Enc.Decrypt(categoryId);

            // Cache categories (rarely change)
            var cats = await _cache.GetOrCreateAsync("template_categories", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _catCacheTtl;
                return await UOF.TemplateRepo.GetCategoriesAsync();
            }) ?? new List<TemplateCategoryDTO>();

            // Cache template list per page/filter combo
            var cacheKey = $"templates_{realCatId}_{orientation}_{searchKey}_{pageNo}";
            var paged = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _listCacheTtl;
                return await UOF.TemplateRepo.GetListAsync(
                    "ACTIVE", realCatId, orientation, searchKey, pageNo, 12);
            });

            // Encrypt IDs
            if (paged?.Items != null)
                foreach (var t in paged.Items) t.EncId = Enc.Encrypt(t.TemplateId);
            foreach (var c in cats) c.EncId = Enc.Encrypt(c.CategoryId);

            var vm = new BannerTemplateDTO
            {
                TemplateList = paged?.Items ?? new(),
                CategoryList = cats,
                SearchKey    = searchKey,
                PageNo       = pageNo,
                TotalPages   = paged?.TotalPages   ?? 0,
                TotalRecords = paged?.TotalRecord   ?? 0
            };
            return View(vm);
        }

        public async Task<IActionResult> Preview(string id)
        {
            var realId = Enc.Decrypt(id);
            if (realId == null) return NotFound();

            var cacheKey = $"template_{realId}";
            var model = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return await UOF.TemplateRepo.GetByIdAsync(realId.Value);
            });

            if (model == null) return NotFound();
            model.EncId = Enc.Encrypt(model.TemplateId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UseTemplate(string templateId)
        {
            var realId = Enc.Decrypt(templateId);
            if (realId.HasValue)
                await UOF.TemplateRepo.IncrementUsageAsync(realId.Value);
            return RedirectToAction("Create", "Design", new { templateId });
        }
    }
}
