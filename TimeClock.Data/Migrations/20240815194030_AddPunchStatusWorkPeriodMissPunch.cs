using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeClock.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPunchStatusWorkPeriodMissPunch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            DefaultData.SeedDbFunctions(migrationBuilder, nameof(TimeClockContext.GetPunchStatus));
            DefaultData.SeedDbFunctions(migrationBuilder, nameof(TimeClockContext.GetStablePunchStatus));
            DefaultData.SeedDbFunctions(migrationBuilder, nameof(TimeClockContext.IsPreviousMissingPunch));

            migrationBuilder.AddColumn<bool>(
                name: "IsPreviousMissingPunch",
                schema: "timeclock",
                table: "WorkPeriods",
                type: "bit",
                nullable: true,
                computedColumnSql: "[timeclock].[IsPreviousMissingPunch]([Id])",
                stored: false);

            migrationBuilder.AddColumn<string>(
                name: "StableStatus",
                schema: "timeclock",
                table: "PunchEntriesCurrentStates",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true,
                computedColumnSql: "[timeclock].[GetStablePunchStatus]([Id])",
                stored: false);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "timeclock",
                table: "PunchEntriesCurrentStates",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                computedColumnSql: "[timeclock].[GetPunchStatus]([Id])",
                stored: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPreviousMissingPunch",
                schema: "timeclock",
                table: "WorkPeriods");

            migrationBuilder.DropColumn(
                name: "StableStatus",
                schema: "timeclock",
                table: "PunchEntriesCurrentStates");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "timeclock",
                table: "PunchEntriesCurrentStates");

            migrationBuilder.Sql("DROP FUNCTION timeclock.GetPunchStatus");
            migrationBuilder.Sql("DROP FUNCTION timeclock.GetStablePunchStatus");
            migrationBuilder.Sql("DROP FUNCTION timeclock.IsPreviousMissingPunch");
        }
    }
}
