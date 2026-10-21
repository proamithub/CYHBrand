using CYHBrandData.Entities;
using CYHBrandModel.Common;
using CYHBrandModel.DTO;

namespace CYHBrandRepo.IRepository
{
    public interface IDesignRepository : IRepository<BannerDesign>
    {
        Task<PagedResult<BannerDesignDTO>> GetListAsync(int doctorId, string? status, string? searchKey, int pageNo, int pageSize);
        Task<BannerDesignDTO?>             GetByIdAsync(int designId, int doctorId);
        Task<ApiResponse>                  SaveAsync(BannerDesignDTO model);
        Task<ApiResponse>                  DeleteAsync(int designId, int doctorId);
        Task<ApiResponse>                  UpdateStatusAsync(int designId, string status, int doctorId);
    }
}
