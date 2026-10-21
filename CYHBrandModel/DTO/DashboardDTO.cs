namespace CYHBrandModel.DTO
{
    public class DashboardDTO
    {
        public int TotalDesigns { get; set; }
        public int TotalPublished { get; set; }
        public int TotalScheduled { get; set; }
        public int TotalTemplates { get; set; }
        public int ProfileCompletionPercent { get; set; }
        public List<BannerDesignDTO> RecentDesigns { get; set; } = new();
        public List<ScheduledPostDTO> UpcomingPosts { get; set; } = new();
    }
}
