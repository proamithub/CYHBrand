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
    public class TemplateRepository : Repository<BannerTemplate>, ITemplateRepository
    {
        private readonly string _conn;
        private readonly ILogger<TemplateRepository> _logger;

        public TemplateRepository(DataContext db, IDapperContext dapper, ILogger<TemplateRepository> logger)
            : base(db)
        {
            _conn   = dapper.ConnectionString;
            _logger = logger;
        }

        public async Task<PagedResult<BannerTemplateDTO>> GetListAsync(
            string flag, int? categoryId, string? orientation, string? searchKey, int pageNo, int pageSize)
        {
            try
            {
                await using var con = new SqlConnection(_conn);
                var param = new { Flag = flag, CategoryId = categoryId, Orientation = orientation,
                                  SearchKey = searchKey, PageNo = pageNo, PageSize = pageSize };
                var rows  = await con.QueryAsync<BannerTemplateDTO>(
                    "GetTemplates_CYH", param, commandType: CommandType.StoredProcedure);
                var list  = rows.ToList();
                return new PagedResult<BannerTemplateDTO> { Items = list, TotalRecord = list.Count, PageNo = pageNo, PageSize = pageSize };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TemplateRepository.GetListAsync failed");
                return new PagedResult<BannerTemplateDTO> { PageNo = pageNo, PageSize = pageSize };
            }
        }

        public async Task<BannerTemplateDTO?> GetByIdAsync(int templateId)
        {
            try
            {
                await using var con = new SqlConnection(_conn);
                var param  = new { Flag = "GET_BY_ID", TemplateId = templateId };
                var result = await con.QueryAsync<BannerTemplateDTO>(
                    "GetTemplates_CYH", param, commandType: CommandType.StoredProcedure);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TemplateRepository.GetByIdAsync failed for TemplateId={Id}", templateId);
                return null;
            }
        }

        public async Task<List<TemplateCategoryDTO>> GetCategoriesAsync()
        {
            try
            {
                await using var con = new SqlConnection(_conn);
                var param  = new { Flag = "CATEGORIES" };
                var result = await con.QueryAsync<TemplateCategoryDTO>(
                    "GetTemplates_CYH", param, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TemplateRepository.GetCategoriesAsync failed");
                return new List<TemplateCategoryDTO>();
            }
        }

        public async Task<ApiResponse> SaveAsync(BannerTemplateDTO model, int adminId)
        {
            try
            {
                await using var con = new SqlConnection(_conn);
                var param = new
                {
                    Flag          = model.TemplateId > 0 ? "UPDATE" : "INSERT",
                    TemplateId    = model.TemplateId,
                    CategoryId    = model.CategoryId,
                    TemplateName  = model.TemplateName,
                    Description   = model.Description,
                    Tags          = model.Tags,
                    ThumbnailPath = model.ThumbnailPath,
                    TemplateJson  = model.TemplateJson,
                    Orientation   = model.Orientation,
                    WidthPx       = model.WidthPx,
                    HeightPx      = model.HeightPx,
                    IsPremium     = model.IsPremium,
                    CreatedBy     = adminId
                };
                var result = await con.QueryFirstOrDefaultAsync<SpResult>(
                    "ManageTemplate_CYH", param, commandType: CommandType.StoredProcedure);
                return result?.ToApiResponse() ?? ApiResponse.FailResult("Save template failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TemplateRepository.SaveAsync failed");
                return ApiResponse.ServerErrorResult(ex.Message);
            }
        }

        public async Task<ApiResponse> ToggleStatusAsync(int templateId, bool isActive, int updatedBy)
        {
            try
            {
                await using var con = new SqlConnection(_conn);
                var param  = new { Flag = "TOGGLE_STATUS", TemplateId = templateId, IsActive = isActive, UpdatedBy = updatedBy };
                var result = await con.QueryFirstOrDefaultAsync<SpResult>(
                    "ManageTemplate_CYH", param, commandType: CommandType.StoredProcedure);
                return result?.ToApiResponse() ?? ApiResponse.FailResult("Toggle failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TemplateRepository.ToggleStatusAsync failed for TemplateId={Id}", templateId);
                return ApiResponse.ServerErrorResult(ex.Message);
            }
        }

        public async Task<ApiResponse> DeleteAsync(int templateId, int deletedBy)
        {
            try
            {
                await using var con = new SqlConnection(_conn);
                var param  = new { Flag = "DELETE", TemplateId = templateId, DeletedBy = deletedBy };
                var result = await con.QueryFirstOrDefaultAsync<SpResult>(
                    "ManageTemplate_CYH", param, commandType: CommandType.StoredProcedure);
                return result?.ToApiResponse() ?? ApiResponse.FailResult("Delete failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TemplateRepository.DeleteAsync failed for TemplateId={Id}", templateId);
                return ApiResponse.ServerErrorResult(ex.Message);
            }
        }

        public async Task IncrementUsageAsync(int templateId)
        {
            try
            {
                await using var con = new SqlConnection(_conn);
                var param = new { Flag = "INCREMENT_USAGE", TemplateId = templateId };
                await con.ExecuteAsync("ManageTemplate_CYH", param, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TemplateRepository.IncrementUsageAsync failed for TemplateId={Id}", templateId);
            }
        }
    }
}
