using CYHBrandModel.DTO;
using CYHBrandUI.Helpers;

namespace CYHBrandUI.Services
{
    public interface IAiBannerService
    {
        Task<AiBannerResponseDTO> GenerateBannerAsync(AiBannerRequestDTO request, CustomPrincipal? doctor);
    }
}
