namespace CYHBrandModel.DTO
{
    public class BannerTemplateDTO
    {
        public int TemplateId { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? TemplateName { get; set; }
        public string? Description { get; set; }
        public string? Tags { get; set; }
        public string? ThumbnailPath { get; set; }
        public string? TemplateJson { get; set; }
        public string? Orientation { get; set; }
        public int? WidthPx { get; set; }
        public int? HeightPx { get; set; }
        public bool IsPremium { get; set; }
        public bool IsActive { get; set; }
        public int UsageCount { get; set; }
        public DateTime? CreatedDate { get; set; }

        // Encrypted ID for URLs
        public string? EncId { get; set; }

        // For listing page / view model
        public List<BannerTemplateDTO>? TemplateList { get; set; }
        public List<TemplateCategoryDTO>? CategoryList { get; set; }
        public string? SearchKey { get; set; }
        public int PageNo { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
    }

    public class TemplateCategoryDTO
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Slug { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public string? EncId { get; set; }   // encrypted ID for URLs
    }
}
