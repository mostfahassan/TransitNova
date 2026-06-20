namespace TransitNovaUI.BusinessLayer.Common.ResultPattern;

public sealed class UiPagedResult<T>
{
    public IEnumerable<T> Data { get; set; } = [];

    public int TotalCount { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public static UiPagedResult<T> From(
        IEnumerable<T> data,
        int totalCount,
        int pageNumber,
        int pageSize) =>
        new()
        {
            Data = data,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
}
