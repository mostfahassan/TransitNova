namespace TransitNova.BusinessLayer.Common.ResultPattern
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; } = [];
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        private PagedResult(IEnumerable<T> data, int totalCount, int pageNumber, int pageSize)
        {
            Data = data;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public static PagedResult<T> From(IEnumerable<T> data, int totalCount, int pageNumber, int pageSize) =>
            new(data, totalCount, pageNumber, pageSize);
           
    }
}
