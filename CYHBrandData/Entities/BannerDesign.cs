using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CYHBrandData.Entities
{
    [Table("BannerDesign")]
    public class BannerDesign
    {
        [Key]
        public int DesignId { get; set; }
        public int? DoctorId { get; set; }
        public string? DesignName { get; set; }
        public int? SourceTemplateId { get; set; }
        public string? DesignJson { get; set; }
        public string? ThumbnailPath { get; set; }
        public string? ExportedFilePath { get; set; }
        public string Status { get; set; } = "Draft";    // Draft | Saved | Published
        public string? Orientation { get; set; }
        public int? WidthPx { get; set; }
        public int? HeightPx { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
    }
}
