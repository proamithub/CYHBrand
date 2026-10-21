namespace CYHBrandModel.Common
{
    /// <summary>Wraps a paged list result from any list SP.</summary>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalRecord { get; set; }
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalRecord / PageSize) : 0;
        public bool HasNextPage => PageNo < TotalPages;
        public bool HasPreviousPage => PageNo > 1;
    }

    /// <summary>Common paging + filter params passed to SP.</summary>
    public class PagingRequest
    {
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchKey { get; set; }
        public string? SortBy { get; set; }
        public string SortDir { get; set; } = "ASC";
    }
}
