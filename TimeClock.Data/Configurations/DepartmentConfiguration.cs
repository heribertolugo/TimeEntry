using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(Department)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(d => d.RowId).IsClustered(true);

        builder.HasIndex(d => d.Name, "IX_Departments").IsUnique();

        builder.Property(d => d.IsActive).HasDefaultValue(true);
        builder.Property(d => d.Name).HasMaxLength(50).IsUnicode(false);
        builder.Property(d => d.RowId).ValueGeneratedOnAdd();
        builder.Property(d => d.Id).HasDefaultValueSql("NEWID()");
        builder.Property(d => d.JdeId).IsRequired(false);
    }
}
