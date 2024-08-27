namespace TimeClock.Data
{
    public interface IPaging
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public sealed class Paging : IPaging
    {
        public int PageNumber { get; set; } = 0;
        public int PageSize { get; set; } = 0;
    }
}
