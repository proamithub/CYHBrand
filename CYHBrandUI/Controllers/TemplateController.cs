using CYHBrandModel.DTO;
using CYHBrandRepo;
using CYHBrandUI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CYHBrandUI.Controllers
{
    [Authorize]
    public class TemplateController : BaseController
    {
        public TemplateController(IHttpContextAccessor http, IConfiguration config,
                                  IUnitOfWork uof, IUrlEncryptor enc)
            : base(http, config, uof, enc) { }

        public async Task<IActionResult> Index(string? categoryId, string? orientation, string? searchKey, int pageNo = 1)
        {
            int? realCatId = null;
            if (!string.IsNullOrEmpty(categoryId)) realCatId = Enc.Decrypt(categoryId);

            var paged = await UOF.TemplateRepo.GetListAsync("ACTIVE", realCatId, orientation, searchKey, pageNo, 12);
            var cats  = await UOF.TemplateRepo.GetCategoriesAsync();

            // Encrypt template IDs
            foreach (var t in paged.Items) t.EncId = Enc.Encrypt(t.TemplateId);
            foreach (var c in cats)         c.EncId = Enc.Encrypt(c.CategoryId);

            var vm = new BannerTemplateDTO
            {
                TemplateList = paged.Items,
                CategoryList = cats,
                SearchKey    = searchKey,
                PageNo       = pageNo,
                TotalPages   = paged.TotalPages,
                TotalRecords = paged.TotalRecord
            };
            return View(vm);
        }

        public async Task<IActionResult> Preview(string id)
        {
            var realId = Enc.Decrypt(id);
            if (realId == null) return NotFound();
            var model = await UOF.TemplateRepo.GetByIdAsync(realId.Value);
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
