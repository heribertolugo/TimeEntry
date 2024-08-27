using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeClock.Data.Migrations
{
    /// <inheritdoc />
    public partial class WorkPeriodJobTypeStepSiblings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkPeriodJobTypeSteps",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActiveSince = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeactivatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WorkPeriodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    JobStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PunchEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EquipmentsToUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeactivatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkPeriodJobTypeSteps", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_WorkPeriodJobTypeSteps_RowId", x => x.RowId)
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_WorkPeriodJobTypes_EquipmentsToUsers",
                        column: x => x.EquipmentsToUserId,
                        principalSchema: "timeclock",
                        principalTable: "EquipmentsToUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkPeriodJobTypes_JobSteps",
                        column: x => x.JobStepId,
                        principalSchema: "timeclock",
                        principalTable: "JobSteps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkPeriodJobTypes_JobTypes",
                        column: x => x.JobTypeId,
                        principalSchema: "timeclock",
                        principalTable: "JobTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkPeriodJobTypes_PunchEntries",
                        column: x => x.PunchEntryId,
                        principalSchema: "timeclock",
                        principalTable: "PunchEntries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkPeriodJobTypes_Users_DeactivatedBy",
                        column: x => x.DeactivatedById,
                        principalSchema: "timeclock",
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkPeriodJobTypes_WorkPeriods",
                        column: x => x.WorkPeriodId,
                        principalSchema: "timeclock",
                        principalTable: "WorkPeriods",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkPeriodJobTypeSteps_DeactivatedById",
                schema: "timeclock",
                table: "WorkPeriodJobTypeSteps",
                column: "DeactivatedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPeriodJobTypeSteps_EquipmentsToUserId",
                schema: "timeclock",
                table: "WorkPeriodJobTypeSteps",
                column: "EquipmentsToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPeriodJobTypeSteps_JobStepId",
                schema: "timeclock",
                table: "WorkPeriodJobTypeSteps",
                column: "JobStepId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPeriodJobTypeSteps_JobTypeId",
                schema: "timeclock",
                table: "WorkPeriodJobTypeSteps",
                column: "JobTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPeriodJobTypeSteps_PunchEntryId",
                schema: "timeclock",
                table: "WorkPeriodJobTypeSteps",
                column: "PunchEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPeriodJobTypeSteps_WorkPeriodId",
                schema: "timeclock",
                table: "WorkPeriodJobTypeSteps",
                column: "WorkPeriodId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkPeriodJobTypeSteps",
                schema: "timeclock");
        }
    }
}
