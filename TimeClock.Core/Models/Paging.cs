namespace TimeClock.Core.Models;

public interface IPagingDto
{
    int PageSize { get; }
    int PageNumber { get; }
}

public class PagingDto : IPagingDto
{
    public PagingDto(int pageNumber, int pageSize)
    {
        this.PageSize = pageSize;
        this.PageNumber = pageNumber;
    }
    public int PageSize { get; }
    public int PageNumber { get; }
}
