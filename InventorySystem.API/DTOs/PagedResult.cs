namespace InventorySystem.API.DTOs
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        // if we have 25 items and page size is 10 => 25 / 10 = 2.5 so we use Math.Ceiling() to get 3 pages
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / (PageSize > 0 ? PageSize : 1)); 
    }
}
