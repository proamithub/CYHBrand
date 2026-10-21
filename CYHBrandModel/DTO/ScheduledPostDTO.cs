namespace CYHBrandModel.DTO
{
    public class ScheduledPostDTO
    {
        public int PostId { get; set; }
        public int? DoctorId { get; set; }
        public int? DesignId { get; set; }
        public string? DesignName { get; set; }
        public string? ThumbnailPath { get; set; }
        public string? Platform { get; set; }
        public string? Caption { get; set; }
        public string? Status { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string? PlatformPostId { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
