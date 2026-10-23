using CYHBrandModel.DTO;
using CYHBrandRepo;
using CYHBrandUI.Helpers;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace CYHBrandUI.Areas.Admin.Controllers
{
    public class DashboardController : AdminBaseController
    {
        public DashboardController(IHttpContextAccessor http, IConfiguration config,
                                   IUnitOfWork uof, IUrlEncryptor enc)
            : base(http, config, uof, enc)
        {
        }

        public async Task<IActionResult> Index()
        {
            var connStr = _config.GetConnectionString("DefaultConnection")!;
            await using var con = new SqlConnection(connStr);

            var totalDoctors = await con.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM DoctorMaster WHERE RoleId = 1");

            var totalTemplates = await con.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM BannerTemplate WHERE IsActive = 1");

            var totalDesigns = await con.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM BannerDesign");

            var totalCategories = await con.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM TemplateCategoryMaster WHERE IsActive = 1");

            var recentDoctors = (await con.QueryAsync<DoctorMasterDTO>(@"
                SELECT TOP 6 
                    DoctorId, FullName, DisplayName, Email, MobileNo, 
                    Specialty, ClinicName, IsActive, CreatedDate, AiCredits
                FROM DoctorMaster 
                WHERE RoleId = 1
                ORDER BY CreatedDate DESC")).ToList();

            var recentTemplates = (await con.QueryAsync<BannerTemplateDTO>(@"
                SELECT TOP 4 
                    TemplateId, TemplateName, ThumbnailPath, Orientation, 
                    WidthPx, HeightPx, UsageCount, CreatedDate
                FROM BannerTemplate
                WHERE IsActive = 1
                ORDER BY CreatedDate DESC")).ToList();

            ViewBag.TotalDoctors = totalDoctors;
            ViewBag.TotalTemplates = totalTemplates;
            ViewBag.TotalDesigns = totalDesigns;
            ViewBag.TotalCategories = totalCategories;
            ViewBag.RecentDoctors = recentDoctors;
            ViewBag.RecentTemplates = recentTemplates;

            return View();
        }
    }
}
