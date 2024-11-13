namespace Dfe.Sww.Ecf.Core.Models.Pagination;

public class PaginationResult<T>
{
    public required IEnumerable<T> Records { get; set; }

    public required PaginationMetaData MetaData { get; set; }
}
