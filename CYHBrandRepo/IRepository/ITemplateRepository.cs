using CYHBrandData.Entities;
using CYHBrandModel.Common;
using CYHBrandModel.DTO;

namespace CYHBrandRepo.IRepository
{
    public interface ITemplateRepository : IRepository<BannerTemplate>
    {
        Task<PagedResult<BannerTemplateDTO>> GetListAsync(string flag, int? categoryId, string? orientation, string? searchKey, int pageNo, int pageSize);
        Task<BannerTemplateDTO?>             GetByIdAsync(int templateId);
        Task<List<TemplateCategoryDTO>>      GetCategoriesAsync();
        Task<ApiResponse>                    SaveAsync(BannerTemplateDTO model, int adminId);
        Task<ApiResponse>                    ToggleStatusAsync(int templateId, bool isActive, int updatedBy);
        Task<ApiResponse>                    DeleteAsync(int templateId, int deletedBy);
        Task                                 IncrementUsageAsync(int templateId);
    }
}
