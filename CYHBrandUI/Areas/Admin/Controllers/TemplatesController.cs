using CYHBrandModel.DTO;
using CYHBrandRepo;
using CYHBrandUI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace CYHBrandUI.Areas.Admin.Controllers
{
    public class TemplatesController : AdminBaseController
    {
        public TemplatesController(IHttpContextAccessor http, IConfiguration config,
                                   IUnitOfWork uof, IUrlEncryptor enc)
            : base(http, config, uof, enc)
        {
        }

        public async Task<IActionResult> Index(int? categoryId, string? orientation, string? searchKey, int pageNo = 1)
        {
            var cats = await UOF.TemplateRepo.GetCategoriesAsync();
            var paged = await UOF.TemplateRepo.GetListAsync(
                "ACTIVE", categoryId, orientation, searchKey, pageNo, 20);

            var vm = new BannerTemplateDTO
            {
                TemplateList = paged?.Items ?? new(),
                CategoryList = cats ?? new(),
                CategoryId   = categoryId,
                Orientation  = orientation,
                SearchKey    = searchKey,
                PageNo       = pageNo,
                TotalPages   = paged?.TotalPages ?? 0,
                TotalRecords = paged?.TotalRecord ?? 0
            };

            return View(vm);
        }
    }
}
