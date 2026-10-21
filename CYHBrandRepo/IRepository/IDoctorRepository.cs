using CYHBrandData.Entities;
using CYHBrandModel.Common;
using CYHBrandModel.DTO;

namespace CYHBrandRepo.IRepository
{
    public interface IDoctorRepository : IRepository<DoctorMaster>
    {
        // ── Auth ──────────────────────────────────────────────────────────
        Task<DoctorMasterDTO?> AuthenticateAsync(string email, string password);
        Task<ApiResponse>      RegisterAsync(DoctorMasterDTO model);
        Task<bool>             EmailExistsAsync(string email);

        // ── Profile ───────────────────────────────────────────────────────
        Task<DoctorMasterDTO?> GetByIdAsync(int doctorId);
        Task<ApiResponse>      UpdateProfileAsync(DoctorMasterDTO model);
        Task<ApiResponse>      UpdateBrandSettingsAsync(DoctorMasterDTO model);
        Task<ApiResponse>      UpdateProfilePicAsync(int doctorId, string filePath);
        Task<ApiResponse>      UpdateClinicLogoAsync(int doctorId, string filePath);

        // ── Admin ─────────────────────────────────────────────────────────
        Task<PagedResult<DoctorMasterDTO>> GetListAsync(string? searchKey, int pageNo, int pageSize);
        Task<ApiResponse>                  ToggleStatusAsync(int doctorId, bool isActive, int updatedBy);
    }
}
