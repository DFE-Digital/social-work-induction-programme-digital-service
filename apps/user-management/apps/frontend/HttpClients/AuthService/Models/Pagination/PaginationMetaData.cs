namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;

public class PaginationMetaData
{
    public int Offset { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int PageCount { get; set; }
    public Dictionary<string, MetaDataLink>? Links { get; set; }
}

public class MetaDataLink
{
    public int Offset { get; set; }
    public int PageSize { get; set; }
}
