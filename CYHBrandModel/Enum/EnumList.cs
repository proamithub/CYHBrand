namespace CYHBrandModel.Enum
{
    public enum UserRole
    {
        Doctor    = 1,
        Admin     = 2,
        SuperAdmin = 3
    }

    public enum DesignStatus
    {
        Draft,
        Saved,
        Published
    }

    public enum PostStatus
    {
        Pending,
        Scheduled,
        Published,
        Failed,
        Cancelled
    }

    public enum BannerOrientation
    {
        Square,
        Landscape,
        Portrait
    }
}
