namespace ECommerceProductsAPI.Dtos;

public class PaginatedResponse<T>
{
    public List<T> Items { get; set; } = [];
    public int CurrentPageNumber { get; set; }
    public int TotalPages { get; set; }
    public int ItemsPerPage { get; set; }
    public int TotalItems { get; set; }
}
