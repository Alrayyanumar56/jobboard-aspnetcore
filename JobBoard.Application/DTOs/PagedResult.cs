namespace JobBoard.Application.DTOs
{
    // One page of a longer list. TotalCount is the number of matching rows across ALL pages,
    // so a client can show "page 2 of 7" without fetching everything.
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
