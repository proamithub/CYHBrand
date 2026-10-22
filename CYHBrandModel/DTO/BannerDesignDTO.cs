namespace CYHBrandModel.DTO
{
    public class BannerDesignDTO
    {
        public int      DesignId          { get; set; }
        public int?     DoctorId          { get; set; }
        public string?  DesignName        { get; set; }
        public int?     SourceTemplateId  { get; set; }
        public string?  TemplateName      { get; set; }
        public string?  CategoryName      { get; set; }
        public string?  DesignJson        { get; set; }
        public string?  ThumbnailPath     { get; set; }
        public string?  ExportedFilePath  { get; set; }
        public string?  Status            { get; set; }
        public int      StatusId          { get; set; }   // 1=Draft, 2=Published, 3=Scheduled
        public string?  Orientation       { get; set; }
        public int?     WidthPx           { get; set; }
        public int?     HeightPx          { get; set; }
        public DateTime CreatedDate       { get; set; }
        public DateTime? UpdatedDate      { get; set; }
        public DateTime CreatedAt         { get => CreatedDate; set => CreatedDate = value; }
        public DateTime UpdatedAt         { get => (UpdatedDate.HasValue && UpdatedDate.Value != default) ? UpdatedDate.Value : CreatedDate; set => UpdatedDate = value; }

        // Encrypted ID for URLs (not stored in DB)
        public string?  EncId             { get; set; }

        // For listing page / view model
        public List<BannerDesignDTO>? DesignList   { get; set; }
        public string?  FilterStatus      { get; set; }
        public string?  SearchKey         { get; set; }
        public int      PageNo            { get; set; } = 1;
        public int      TotalPages        { get; set; }
        public int      TotalRecords      { get; set; }
    }
}
