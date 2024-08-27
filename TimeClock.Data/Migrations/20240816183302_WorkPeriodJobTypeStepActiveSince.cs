using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeClock.Data.Migrations
{
    /// <inheritdoc />
    public partial class WorkPeriodJobTypeStepActiveSince : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            DefaultData.SeedDbFunctions(migrationBuilder, nameof(TimeClockContext.WorkPeriodJobTypeStepActiveSince));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ActiveSince",
                schema: "timeclock",
                table: "WorkPeriodJobTypeSteps",
                type: "datetime2",
                nullable: false,
                computedColumnSql: "[timeclock].[WorkPeriodJobTypeStepActiveSince]([PunchEntryId], [EquipmentsToUserId], [ActivatedOn])",
                stored: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ActiveSince",
                schema: "timeclock",
                table: "WorkPeriodJobTypeSteps",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComputedColumnSql: "[timeclock].[WorkPeriodJobTypeStepActiveSince]([PunchEntryId], [EquipmentsToUserId], [ActivatedOn])");

            migrationBuilder.Sql("DROP FUNCTION timeclock.WorkPeriodJobTypeStepActiveSince");
        }
    }
}
