using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models.Jde;

namespace TimeClock.Data.Configurations.Jde;

internal class JdeEmployeeConfiguration : IEntityTypeConfiguration<JdeEmployee>
{
    public void Configure(EntityTypeBuilder<JdeEmployee> builder)
    {
        builder.HasKey(e => e.Id).HasName("F060116_PK");

        builder.ToTable("F060116", CommonValues.JdeSchema);

        //builder.HasIndex(e => new { e.Yatinc, e.Yaanpa, e.Yanrvw }, "F060116_10");
        //builder.HasIndex(e => new { e.Yahmcu, e.Yamail, e.Yaan8 }, "F060116_11").IsUnique();
        builder.HasIndex(e => new { e.SupervisorId, e.Id }, "F060116_12").IsUnique();
        builder.HasIndex(e => new { e.Id, e.SupervisorId }, "F060116_13").IsUnique();
        builder.HasIndex(e => new { e.SupervisorId, e.Id }, "F060116_12").IsUnique();
        builder.HasIndex(e => new { e.Id, e.SupervisorId }, "F060116_13").IsUnique();
        builder.HasIndex(e => new { e.Id, e.JobType, e.JobStep }, "F060116_14");
        builder.HasIndex(e => e.EmployeeId, "F060116_15");
        builder.HasIndex(e => new { e.Company, e.UnionCode, e.LocationId, e.Id }, "F060116_17");
        builder.HasIndex(e => new { e.Company, e.UnionCode, e.LocationId, e.Id }, "F060116_17");
        //builder.HasIndex(e => e.Yassn, "F060116_2");
        //builder.HasIndex(e => new { e.Yausr, e.Yauflg }, "F060116_7");
        //builder.HasIndex(e => new { e.Yanrvw, e.Yaanpa, e.Yatinc }, "F060116_8");
        //builder.HasIndex(e => new { e.Yaanpa, e.Yatinc, e.Yanrvw }, "F060116_9");

        builder.Property(e => e.Id)
            .HasColumnType("NUMBER")
            .HasColumnName("YAAN8");
        builder.Property(e => e.Name)
            .HasMaxLength(40)
            .IsFixedLength()
            .HasColumnName("YAALPH");
        builder.Property(e => e.SupervisorId)
            .HasColumnType("NUMBER")
            .HasColumnName("YAANPA");
        builder.Property(e => e.LocationId)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("YAHMCU");
        builder.Property(e => e.JobType)
            .HasMaxLength(6)
            .IsFixedLength()
            .HasColumnName("YAJBCD");
        builder.Property(e => e.JobStep)
            .HasMaxLength(4)
            .IsFixedLength()
            .HasColumnName("YAJBST");
        builder.Property(e => e.PayStatus)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YAPAST");
        builder.Property(e => e.EmployeeId)
            .HasMaxLength(8)
            .IsFixedLength()
            .HasColumnName("YAOEMP");
        builder.Property(e => e.Company)
            .HasMaxLength(5)
            .IsFixedLength()
            .HasColumnName("YAHMCO");
        builder.Property(e => e.UnionCode)
            .HasMaxLength(6)
            .IsFixedLength()
            .HasColumnName("YAUN");

        builder.HasOne(m => m.Location)
            .WithMany()
            .HasPrincipalKey(l => l.BusinessUnit)
            .HasForeignKey(m => m.LocationId);
        builder.HasOne(m => m.Supervisor)
            .WithMany(s => s.Subordinates)
            .HasPrincipalKey(m => m.Id)
            .HasForeignKey(m => m.SupervisorId);
        builder.HasMany(e => e.Emails)
            .WithOne(m => m.Employee)
            .HasForeignKey(m => m.EmployeeId);
    }
}
