namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;

public class PaginationRequest
{
    public PaginationRequest(int? offset, int? pageSize)
    {
        Offset = offset ?? 0;
        PageSize = pageSize ?? 10;
    }

    public int Offset { get; set; }
    public int PageSize { get; set; }
}
