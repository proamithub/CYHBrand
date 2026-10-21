using CYHBrandData;
using CYHBrandData.Dapper;
using CYHBrandData.Entities;
using CYHBrandModel.Common;
using CYHBrandModel.DTO;
using CYHBrandRepo.IRepository;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace CYHBrandRepo.Repository
{
    public class DesignRepository : Repository<BannerDesign>, IDesignRepository
    {
        private readonly string _conn;
        private readonly ILogger<DesignRepository> _logger;

        public DesignRepository(DataContext db, IDapperContext dapper, ILogger<DesignRepository> logger)
            : base(db)
        {
            _conn   = dapper.ConnectionString;
            _logger = logger;
        }

        public async Task<PagedResult<BannerDesignDTO>> GetListAsync(
            int doctorId, string? status, string? searchKey, int pageNo, int pageSize)
        {
            try
            {
                await using var con = new SqlConnection(_conn);
                var param = new { Flag = "LIST", DoctorId = doctorId, Status = status,
                                  SearchKey = searchKey, PageNo = pageNo, PageSize = pageSize };
                var rows  = await con.QueryAsync<BannerDesignDTO>(
                    "GetDesigns_CYH", param, commandType: CommandType.StoredProcedure);
                var list  = rows.ToList();
                return new PagedResult<BannerDesignDTO> { Items = list, TotalRecord = list.Count, PageNo = pageNo, PageSize = pageSize };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DesignRepository.GetListAsync failed for DoctorId={Id}", doctorId);
                return new PagedResult<BannerDesignDTO> { PageNo = pageNo, PageSize = pageSize };
            }
        }

        public async Task<BannerDesignDTO?> GetByIdAsync(int designId, int doctorId)
        {
            try
            {
                await using var con = new SqlConnection(_conn);
                var param  = new { Flag = "GET_BY_ID", DesignId = designId, DoctorId = doctorId };
                var result = await con.QueryAsync<BannerDesignDTO>(
                    "GetDesigns_CYH", param, commandType: CommandType.StoredProcedure);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DesignRepository.GetByIdAsync failed for DesignId={Id}", designId);
                return null;
            }
        }

        public async Task<ApiResponse> SaveAsync(BannerDesignDTO model)
        {
            try
            {
                await using var con = new SqlConnection(_conn);
                var param = new
                {
                    Flag             = model.DesignId > 0 ? "UPDATE" : "INSERT",
                    DesignId         = model.DesignId,
                    DoctorId         = model.DoctorId,
                    DesignName       = model.DesignName,
                    SourceTemplateId = model.SourceTemplateId,
                    DesignJson       = model.DesignJson,
                    ThumbnailPath    = model.ThumbnailPath,
                    Status           = model.Status ?? "Draft",
                    Orientation      = model.Orientation,
                    WidthPx          = model.WidthPx,
                    HeightPx         = model.HeightPx
                };
                var result = await con.QueryFirstOrDefaultAsync<SpResult>(
                    "ManageDesign_CYH", param, commandType: CommandType.StoredProcedure);
                return result?.ToApiResponse() ?? ApiResponse.FailResult("Save design failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DesignRepository.SaveAsync failed");
                return ApiResponse.ServerErrorResult(ex.Message);
            }
        }

        public async Task<ApiResponse> DeleteAsync(int designId, int doctorId)
        {
            try
            {
                await using var con = new SqlConnection(_conn);
                var param  = new { Flag = "DELETE", DesignId = designId, DoctorId = doctorId };
                var result = await con.QueryFirstOrDefaultAsync<SpResult>(
                    "ManageDesign_CYH", param, commandType: CommandType.StoredProcedure);
                return result?.ToApiResponse() ?? ApiResponse.FailResult("Delete failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DesignRepository.DeleteAsync failed for DesignId={Id}", designId);
                return ApiResponse.ServerErrorResult(ex.Message);
            }
        }

        public async Task<ApiResponse> UpdateStatusAsync(int designId, string status, int doctorId)
        {
            try
            {
                await using var con = new SqlConnection(_conn);
                var param  = new { Flag = "UPDATE_STATUS", DesignId = designId, Status = status, DoctorId = doctorId };
                var result = await con.QueryFirstOrDefaultAsync<SpResult>(
                    "ManageDesign_CYH", param, commandType: CommandType.StoredProcedure);
                return result?.ToApiResponse() ?? ApiResponse.FailResult("Status update failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DesignRepository.UpdateStatusAsync failed for DesignId={Id}", designId);
                return ApiResponse.ServerErrorResult(ex.Message);
            }
        }
    }
}
