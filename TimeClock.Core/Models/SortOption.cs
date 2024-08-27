namespace TimeClock.Core.Models;
public class SortOption<SortField>
    where SortField : struct, Enum
{
    public SortOption() : this(default, SortOrderDto.Ascending) { }
    public SortOption(SortField field) : this(field, SortOrderDto.Ascending) { }
    public SortOption(SortField field, SortOrderDto sortOrder)
    {
        this.Field = field;
        this.SortOrder = sortOrder;
    }

    public SortField Field { get; set; }
    public SortOrderDto SortOrder { get; set; }
}
