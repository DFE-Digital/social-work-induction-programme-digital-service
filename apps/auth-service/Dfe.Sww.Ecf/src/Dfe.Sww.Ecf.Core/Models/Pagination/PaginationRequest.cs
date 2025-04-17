using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Core.Models.Pagination;

public class PaginationRequest
{
    public PaginationRequest()
    {
    }

    public PaginationRequest(int offset, int pageSize)
    {
        Offset = offset;
        PageSize = pageSize;
    }

    [Required]
    [Range(0, 100000)]
    public int Offset { get; init; }

    [Required]
    [Range(1, 1000)]
    public int PageSize { get; init; }
}
