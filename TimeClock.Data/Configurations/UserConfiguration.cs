using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(User)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(d => d.RowId).IsClustered(true);

        builder.HasIndex(u => u.EmployeeNumber, "IX_Users").IsUnique();
        builder.HasIndex(u => u.UserName, "IX_Users_UserName").IsUnique();
        builder.HasIndex(u => u.JdeId, "IX_Users_JdeId").IsUnique();

        builder.Property(u => u.EmployeeNumber).HasMaxLength(20).IsUnicode(false);
        builder.Property(u => u.FirstName).HasMaxLength(25).IsUnicode(false);
        builder.Property(u => u.IsActive).HasDefaultValue(true);
        builder.Property(u => u.LastName).HasMaxLength(25).IsUnicode(false);
        builder.Property(u => u.UserName).HasMaxLength(40).IsUnicode(false).IsRequired(false);
        builder.Property(u => u.RowId).ValueGeneratedOnAdd();
        builder.Property(u => u.Id).HasDefaultValueSql("NEWID()");
        builder.Property(u => u.FailureCount);
        builder.Property(u => u.LastActionOn).IsRequired(false);
        builder.Property(u => u.LockedOutOn).IsRequired(false);
        builder.Property(d => d.JdeId).IsRequired(false);
        builder.Property(u => u.UnionCode).HasMaxLength(6).IsUnicode(false).IsRequired(false);
        builder.Property(u => u.PrimaryEmail).HasMaxLength(35).IsUnicode(false).IsRequired(false);
        builder.Property(u => u.IsAdmin);

        builder.HasOne(u => u.DepartmentsToLocation).WithMany(d => d.Users)
            .HasForeignKey(u => u.DepartmentsToLocationId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Users_DepartmentsToLocations");
        builder.HasOne(u => u.DefaultJobStep).WithMany()
            .IsRequired(false)
            .HasForeignKey(u => u.DefaultJobTypeId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Users_JobType");
        builder.HasOne(u => u.DefaultJobStep).WithMany()
            .IsRequired(false)
            .HasForeignKey(u => u.DefaultJobStepId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Users_JobSteps");
        builder.HasOne(u => u.Supervisor)
            .WithMany(s => s.Subordinates)
            .HasForeignKey(u => u.SupervisorJdeId)
            .HasPrincipalKey(u => u.JdeId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Users_Users_Supervisors");
    }
}
