using CYHBrandData.Entities;
using Microsoft.EntityFrameworkCore;

namespace CYHBrandData
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<DoctorMaster> DoctorMasters { get; set; }
        public DbSet<RoleMaster> RoleMasters { get; set; }
        public DbSet<BannerTemplate> BannerTemplates { get; set; }
        public DbSet<TemplateCategoryMaster> TemplateCategoryMasters { get; set; }
        public DbSet<BannerDesign> BannerDesigns { get; set; }
        public DbSet<ScheduledPost> ScheduledPosts { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
