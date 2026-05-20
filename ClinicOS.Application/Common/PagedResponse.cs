namespace ClinicOS.Application.Common;

/// <summary>
/// Paged response wrapper for paginated data
/// </summary>
/// <typeparam name="T">Type of data being returned</typeparam>
public class PagedResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<T> Data { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static PagedResponse<T> Create(List<T> data, int pageNumber, int pageSize, int totalRecords, string message = "Data retrieved successfully")
    {
        return new PagedResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords
        };
    }
}
