using CYHBrandData;
using CYHBrandData.Dapper;
using CYHBrandData.Entities;
using CYHBrandModel.Common;
using CYHBrandModel.DTO;
using CYHBrandRepo.IRepository;
using CYHBrandRepo.Utilities;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace CYHBrandRepo.Repository
{
    public class DoctorRepository : Repository<DoctorMaster>, IDoctorRepository
    {
        private readonly string _conn;
        private readonly ILogger<DoctorRepository> _logger;

        public DoctorRepository(DataContext db, IDapperContext dapper, ILogger<DoctorRepository> logger)
            : base(db)
        {
            _conn   = dapper.ConnectionString;
            _logger = logger;
        }

        // ── Auth ──────────────────────────────────────────────────────────

        public async Task<DoctorMasterDTO?> AuthenticateAsync(string email, string password)
        {
            try
            {
                await using var con = new SqlConnection(_conn);
                var param = new { Flag = "LOGIN", Email = email, Password = AESEncryption.Encode(password) };
                var result = await con.QueryAsync<DoctorMasterDTO>(
                    "AuthenticateDoctor_CYH", param, commandType: CommandType.StoredProcedure);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuthenticateAsync failed for {Email}", email);
                return null;
            }
        }

        public async Task<ApiResponse> RegisterAsync(DoctorMasterDTO model)
        {
            try
            {
                await using var con = new SqlConnection(_conn);
                var param = new
                {
                    Flag      = "REGISTER",
                    FullName  = model.FullName,
                    Email     = model.Email,
                    Password  = AESEncryption.Encode(model.Password!),
                    MobileNo  = model.MobileNo,
                    Specialty = model.Specialty
                };
                var result = await con.QueryFirstOrDefaultAsync<SpResult>(
                    "ManageDoctor_CYH", param, commandType: CommandType.StoredProcedure);
                return result?.ToApiResponse() ?? ApiResponse.FailResult("Registration failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RegisterAsync failed for {Email}", model.Email);
                return ApiResponse.ServerErrorResult(ex.Message);
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            try
            {
                await using var con = new SqlConnection(_conn);
                var param  = new { Flag = "EMAIL_CHECK", Email = email };
                var result = await con.QueryFirstOrDefaultAsync<SpResult>(
                    "ManageDoctor_CYH", param, commandType: CommandType.StoredProcedure);
                return result?.IsSuccess == true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EmailExistsAsync failed for {Email}", email);
                return false;
            }
        }

        // ── Profile ───────────────────────────────────────────────────────

        public async Task<DoctorMasterDTO?> GetByIdAsync(int doctorId)
        {
            try
            {
                await using var con = new SqlConnection(_conn);
                var param  = new { Flag = "GET_BY_ID", DoctorId = doctorId };
                var result = await con.QueryAsync<DoctorMasterDTO>(
                    "GetDoctor_CYH", param, commandType: CommandType.StoredProcedure);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync failed for DoctorId={DoctorId}", doctorId);
                return null;
            }
        }

        public async Task<ApiResponse> UpdateProfileAsync(DoctorMasterDTO model)
        {
            try
            {
                await using var con = new SqlConnection(_conn);
                var param = new
                {
                    Flag         = "UPDATE_PROFILE",
                    DoctorId     = model.DoctorId,
                    FullName     = model.FullName,
                    DisplayName  = model.DisplayName,
                    MobileNo     = model.MobileNo,
                    Specialty    = model.Specialty,
                    Qualifications = model.Qualifications,
                    ClinicName   = model.ClinicName,
                    ClinicAddress= model.ClinicAddress,
                    City         = model.City,
                    State        = model.State,
                    PinCode      = model.PinCode,
                    WebsiteUrl   = model.WebsiteUrl,
                    Bio          = model.Bio
                };
                var result = await con.QueryFirstOrDefaultAsync<SpResult>(
                    "ManageDoctor_CYH", param, commandType: CommandType.StoredProcedure);
                return result?.ToApiResponse() ?? ApiResponse.FailResult("Update failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateProfileAsync failed for DoctorId={Id}", model.DoctorId);
                return ApiResponse.ServerErrorResult(ex.Message);
            }
        }

        public async Task<ApiResponse> UpdateBrandSettingsAsync(DoctorMasterDTO model)
        {
            try
            {
                await using var con = new SqlConnection(_conn);
                var param = new
                {
                    Flag               = "UPDATE_BRAND",
                    DoctorId           = model.DoctorId,
                    PrimaryColorHex    = model.PrimaryColorHex,
                    SecondaryColorHex  = model.SecondaryColorHex,
                    PreferredFont      = model.PreferredFont
                };
                var result = await con.QueryFirstOrDefaultAsync<SpResult>(
                    "ManageDoctor_CYH", param, commandType: CommandType.StoredProcedure);
                return result?.ToApiResponse() ?? ApiResponse.FailResult("Brand update failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateBrandSettingsAsync failed for DoctorId={Id}", model.DoctorId);
                return ApiResponse.ServerErrorResult(ex.Message);
            }
        }

        public async Task<ApiResponse> UpdateProfilePicAsync(int doctorId, string filePath)
        {
            try
            {
                await using var con = new SqlConnection(_conn);
                var param  = new { Flag = "UPDATE_PHOTO", DoctorId = doctorId, ProfilePic = filePath };
                var result = await con.QueryFirstOrDefaultAsync<SpResult>(
                    "ManageDoctor_CYH", param, commandType: CommandType.StoredProcedure);
                return result?.ToApiResponse() ?? ApiResponse.FailResult("Photo update failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateProfilePicAsync failed for DoctorId={Id}", doctorId);
                return ApiResponse.ServerErrorResult(ex.Message);
            }
        }

        public async Task<ApiResponse> UpdateClinicLogoAsync(int doctorId, string filePath)
        {
            try
            {
                await using var con = new SqlConnection(_conn);
                var param  = new { Flag = "UPDATE_LOGO", DoctorId = doctorId, ClinicLogo = filePath };
                var result = await con.QueryFirstOrDefaultAsync<SpResult>(
                    "ManageDoctor_CYH", param, commandType: CommandType.StoredProcedure);
                return result?.ToApiResponse() ?? ApiResponse.FailResult("Logo update failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateClinicLogoAsync failed for DoctorId={Id}", doctorId);
                return ApiResponse.ServerErrorResult(ex.Message);
            }
        }

        // ── Admin ─────────────────────────────────────────────────────────

        public async Task<PagedResult<DoctorMasterDTO>> GetListAsync(string? searchKey, int pageNo, int pageSize)
        {
            try
            {
                await using var con = new SqlConnection(_conn);
                var param  = new { Flag = "LIST", SearchKey = searchKey, PageNo = pageNo, PageSize = pageSize };
                var rows   = await con.QueryAsync<DoctorMasterDTO>(
                    "GetDoctor_CYH", param, commandType: CommandType.StoredProcedure);
                var list   = rows.ToList();
                return new PagedResult<DoctorMasterDTO>
                {
                    Items       = list,
                    TotalRecord = list.Count,
                    PageNo      = pageNo,
                    PageSize    = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetListAsync (doctors) failed");
                return new PagedResult<DoctorMasterDTO> { PageNo = pageNo, PageSize = pageSize };
            }
        }

        public async Task<ApiResponse> ToggleStatusAsync(int doctorId, bool isActive, int updatedBy)
        {
            try
            {
                await using var con = new SqlConnection(_conn);
                var param  = new { Flag = "TOGGLE_STATUS", DoctorId = doctorId, IsActive = isActive, UpdatedBy = updatedBy };
                var result = await con.QueryFirstOrDefaultAsync<SpResult>(
                    "ManageDoctor_CYH", param, commandType: CommandType.StoredProcedure);
                return result?.ToApiResponse() ?? ApiResponse.FailResult("Toggle failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ToggleStatusAsync failed for DoctorId={Id}", doctorId);
                return ApiResponse.ServerErrorResult(ex.Message);
            }
        }
    }
}
