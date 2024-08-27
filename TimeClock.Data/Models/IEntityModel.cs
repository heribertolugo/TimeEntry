namespace TimeClock.Data.Models
{
    public interface IEntityModel
    {
        public Guid Id { get; set; }
        public int RowId { get; set; }
    }
}
