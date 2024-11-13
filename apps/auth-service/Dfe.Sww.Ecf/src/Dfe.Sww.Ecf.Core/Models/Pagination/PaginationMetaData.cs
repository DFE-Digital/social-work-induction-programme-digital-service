namespace Dfe.Sww.Ecf.Core.Models.Pagination;

public class PaginationMetaData
{
    public PaginationMetaData(int offset, int pageSize, int totalCount)
    {
        Offset = offset;
        PageSize = pageSize;
        TotalCount = totalCount;
        PageCount = (int)Math.Ceiling(totalCount / (double)pageSize);
        Page = (int)Math.Ceiling((Offset - 1) / (double)PageSize) + 1;
        Links = new Dictionary<string, MetaDataLink>
        {
            { "self", new MetaDataLink { Offset = Offset, PageSize = PageSize } },
            { "first", new MetaDataLink { Offset = 0, PageSize = PageSize } },
            { "previous", new MetaDataLink { Offset = Offset - PageSize < 0 ? 0 : Offset - PageSize, PageSize = PageSize } },
            { "last", new MetaDataLink { Offset = (PageCount - 1) * PageSize, PageSize = PageSize } },
            { "next", new MetaDataLink { Offset = Offset + PageSize > TotalCount ? (PageCount - 1) * pageSize : Offset + PageSize, PageSize = PageSize } },
        };
    }

    public int Offset { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int PageCount { get; set; }
    public Dictionary<string, MetaDataLink> Links { get; set; }
}

public class MetaDataLink
{
    public int Offset { get; set; }
    public int PageSize { get; set; }
}
