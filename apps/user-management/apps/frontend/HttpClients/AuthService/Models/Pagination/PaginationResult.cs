namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;

public class PaginationResult<T>
{
    public required IEnumerable<T> Records { get; set; }

    public required PaginationMetaData MetaData { get; set; }
}
