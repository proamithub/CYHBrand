namespace CYHBrandModel.DTO
{
    public class DashboardDTO
    {
        public int TotalDesigns      { get; set; }
        public int TotalPublished    { get; set; }
        public int ScheduledPosts    { get; set; }
        public int TotalShares       { get; set; }
        public long SocialReach      { get; set; }
        public int TotalTemplates    { get; set; }
        public int ProfileCompletionPercent { get; set; }

        public List<BannerDesignDTO>   RecentDesigns   { get; set; } = new();
        public List<BannerDesignDTO>   TodaysDesigns   { get; set; } = new();
        public List<ScheduledPostDTO>  UpcomingPosts   { get; set; } = new();
    }
}
