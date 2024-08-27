using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TimeClock.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "timeclock");

            migrationBuilder.CreateTable(
                name: "AuthorizationClaims",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    Value = table.Column<string>(type: "varchar(35)", unicode: false, maxLength: 35, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizationClaims", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_AuthorizationClaims_RowId", x => x.RowId)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    JdeId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_Departments_RowId", x => x.RowId)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentTypes",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    JdeId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentTypes", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_EquipmentTypes_RowId", x => x.RowId)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "EventAudits",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    EventName = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    EventDescription = table.Column<string>(type: "varchar(3000)", unicode: false, maxLength: 3000, nullable: true),
                    Success = table.Column<bool>(type: "bit", nullable: false),
                    EntityType = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())", comment: "Date and Time when event occurred. Should be specified in UTC format."),
                    EventDateOnly = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(CONVERT (date, SYSUTCDATETIME()))", comment: "Date only portion of EventDate. Used for creating the indexed value.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAudits", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_EventAudits_RowId", x => x.RowId)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "JobSteps",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    JdeId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSteps", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_JobSteps_RowId", x => x.RowId)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "JobTypes",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    JdeId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTypes", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_JobTypes_RowId", x => x.RowId)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    JdeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DivisionCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_Locations_RowId", x => x.RowId)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "SentEmails",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SentOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SentTo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Signature = table.Column<string>(type: "varchar(512)", unicode: false, maxLength: 512, nullable: false),
                    Subject = table.Column<string>(type: "varchar(60)", unicode: false, maxLength: 60, nullable: false),
                    Message = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SentEmails", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_SentEmails_RowId", x => x.RowId)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "UnregisteredDevices",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    RefreshToken = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnregisteredDevices", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_UnregisteredDevices_RowId", x => x.RowId)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "Equipments",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sku = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    Name = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    EquipmentTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    JdeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_Equipments_RowId", x => x.RowId)
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_Equipments_EquipmentTypes",
                        column: x => x.EquipmentTypeId,
                        principalSchema: "timeclock",
                        principalTable: "EquipmentTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DepartmentsToLocations",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentsToLocations", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_DepartmentsToLocations_RowId", x => x.RowId)
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_DepartmentsToLocations_Departments",
                        column: x => x.DepartmentId,
                        principalSchema: "timeclock",
                        principalTable: "Departments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DepartmentsToLocations_Locations",
                        column: x => x.LocationId,
                        principalSchema: "timeclock",
                        principalTable: "Locations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JobTypeStepToEquipments",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    JobStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnionCode = table.Column<string>(type: "varchar(6)", unicode: false, maxLength: 6, nullable: true),
                    EquipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTypeStepToEquipments", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_JobTypeStepToEquipments_RowId", x => x.RowId)
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_JobTypeStepToEquipments_Equipments",
                        column: x => x.EquipmentId,
                        principalSchema: "timeclock",
                        principalTable: "Equipments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JobTypeStepToEquipments_JobSteps",
                        column: x => x.JobStepId,
                        principalSchema: "timeclock",
                        principalTable: "JobSteps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JobTypeStepToEquipments_JobTypes",
                        column: x => x.JobTypeId,
                        principalSchema: "timeclock",
                        principalTable: "JobTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false),
                    LastName = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false),
                    EmployeeNumber = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    UserName = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    FailureCount = table.Column<int>(type: "int", nullable: false),
                    LastActionOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LockedOutOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UnionCode = table.Column<string>(type: "varchar(6)", unicode: false, maxLength: 6, nullable: true),
                    PrimaryEmail = table.Column<string>(type: "varchar(35)", unicode: false, maxLength: 35, nullable: true),
                    JdeId = table.Column<int>(type: "int", nullable: false),
                    DefaultJobTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DefaultJobStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentsToLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SupervisorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SupervisorJdeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_Users_JdeId", x => x.JdeId);
                    table.UniqueConstraint("AK_Users_RowId", x => x.RowId)
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_Users_DepartmentsToLocations",
                        column: x => x.DepartmentsToLocationId,
                        principalSchema: "timeclock",
                        principalTable: "DepartmentsToLocations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_JobSteps",
                        column: x => x.DefaultJobStepId,
                        principalSchema: "timeclock",
                        principalTable: "JobSteps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_JobTypes_DefaultJobTypeId",
                        column: x => x.DefaultJobTypeId,
                        principalSchema: "timeclock",
                        principalTable: "JobTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_Users_Supervisors",
                        column: x => x.SupervisorJdeId,
                        principalSchema: "timeclock",
                        principalTable: "Users",
                        principalColumn: "JdeId");
                });

            migrationBuilder.CreateTable(
                name: "Barcodes",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    ActivatedOn = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETDATE()"),
                    DeactivatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeactivatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Barcodes", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_Barcodes_RowId", x => x.RowId)
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_Barcodes_Users",
                        column: x => x.UserId,
                        principalSchema: "timeclock",
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Barcodes_Users_DeactivatedBy",
                        column: x => x.DeactivatedById,
                        principalSchema: "timeclock",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    DepartmentsToLocationsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    FailureCount = table.Column<int>(type: "int", nullable: false),
                    LastActionOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LockedOutOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RefreshTokenIssuedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ConfiguredById = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_Devices_RowId", x => x.RowId)
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_Devices_DepartmentsToLocations",
                        column: x => x.DepartmentsToLocationsId,
                        principalSchema: "timeclock",
                        principalTable: "DepartmentsToLocations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Devices_Users",
                        column: x => x.ConfiguredById,
                        principalSchema: "timeclock",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EquipmentsToDepartmentLocations",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentsToLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LinkedOn = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    LinkedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    JdeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentsToDepartmentLocations", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_EquipmentsToDepartmentLocations_RowId", x => x.RowId)
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_EquipmentsToDepartmentLocations_DepartmentsToLocation",
                        column: x => x.DepartmentsToLocationId,
                        principalSchema: "timeclock",
                        principalTable: "DepartmentsToLocations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EquipmentsToDepartmentLocations_Equipments",
                        column: x => x.EquipmentId,
                        principalSchema: "timeclock",
                        principalTable: "Equipments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EquipmentsToDepartmentLocations_Users",
                        column: x => x.LinkedById,
                        principalSchema: "timeclock",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JobTypeStepToUsers",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    JobTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    JobStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTypeStepToUsers", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_JobTypeStepToUsers_RowId", x => x.RowId)
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_JobTypeStepToUsers_JobSteps",
                        column: x => x.JobStepId,
                        principalSchema: "timeclock",
                        principalTable: "JobSteps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JobTypeStepToUsers_JobTypes",
                        column: x => x.JobTypeId,
                        principalSchema: "timeclock",
                        principalTable: "JobTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JobTypeStepToUsers_Users",
                        column: x => x.UserId,
                        principalSchema: "timeclock",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorizationClaimId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_UserClaims_RowId", x => x.RowId)
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_AuthorizationClaims_Users",
                        column: x => x.AuthorizationClaimId,
                        principalSchema: "timeclock",
                        principalTable: "AuthorizationClaims",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserClaims_Users",
                        column: x => x.UserId,
                        principalSchema: "timeclock",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkPeriods",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoursWorked = table.Column<double>(type: "float", nullable: false),
                    WorkDate = table.Column<DateOnly>(type: "date", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Purpose = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    PayPeriodEnd = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkPeriods", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_WorkPeriods_RowId", x => x.RowId);
                    table.ForeignKey(
                        name: "FK_WorkPeriods_Users",
                        column: x => x.UserId,
                        principalSchema: "timeclock",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkPeriodsAudits",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Field = table.Column<string>(type: "varchar(25)", nullable: false),
                    OldValue = table.Column<string>(type: "varchar(25)", nullable: false),
                    NewValue = table.Column<string>(type: "varchar(25)", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkPeriodsAudits", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_WorkPeriodsAudits_RowId", x => x.RowId);
                    table.ForeignKey(
                        name: "FK_WorkPeriodsAudits_Users",
                        column: x => x.UserId,
                        principalSchema: "timeclock",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EquipmentsToUsers",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LinkedOn = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    LinkedOnEffective = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    UnLinkedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UnLinkedOnEffective = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LinkedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnlinkedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkPeriodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    JobStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    JdeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentsToUsers", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_EquipmentsToUsers_RowId", x => x.RowId)
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_EquipmentsToUsers_Equipments",
                        column: x => x.EquipmentId,
                        principalSchema: "timeclock",
                        principalTable: "Equipments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EquipmentsToUsers_JobSteps",
                        column: x => x.JobStepId,
                        principalSchema: "timeclock",
                        principalTable: "JobSteps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EquipmentsToUsers_JobTypes",
                        column: x => x.JobTypeId,
                        principalSchema: "timeclock",
                        principalTable: "JobTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EquipmentsToUsers_Users",
                        column: x => x.UserId,
                        principalSchema: "timeclock",
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EquipmentsToUsers_Users_LinkedBy",
                        column: x => x.LinkedById,
                        principalSchema: "timeclock",
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EquipmentsToUsers_Users_UnlinkedBy",
                        column: x => x.UnlinkedById,
                        principalSchema: "timeclock",
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EquipmentsToUsers_WorkPeriods",
                        column: x => x.WorkPeriodId,
                        principalSchema: "timeclock",
                        principalTable: "WorkPeriods",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PunchEntries",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkPeriodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PunchEntries", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_PunchEntries_RowId", x => x.RowId);
                    table.ForeignKey(
                        name: "FK_PunchEntries_Devices",
                        column: x => x.DeviceId,
                        principalSchema: "timeclock",
                        principalTable: "Devices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PunchEntries_Users",
                        column: x => x.UserId,
                        principalSchema: "timeclock",
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PunchEntries_WorkPeriods",
                        column: x => x.WorkPeriodId,
                        principalSchema: "timeclock",
                        principalTable: "WorkPeriods",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkPeriodStatusHistories",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkPeriodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JdeId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkPeriodStatusHistories", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_WorkPeriodStatusHistories_RowId", x => x.RowId)
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_WorkPeriodStatusHistory_WorkPeriod",
                        column: x => x.WorkPeriodId,
                        principalSchema: "timeclock",
                        principalTable: "WorkPeriods",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PunchEntriesHistories",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PunchEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PunchType = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    EffectiveDateTime = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    Action = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    ActionById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UtcTimeStamp = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())"),
                    Note = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    JobTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    JobStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IndexedDate = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "convert(date, getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PunchEntriesHistories", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_PunchEntriesHistories_RowId", x => x.RowId);
                    table.ForeignKey(
                        name: "FK_PunchEntriesHistories_Devices",
                        column: x => x.DeviceId,
                        principalSchema: "timeclock",
                        principalTable: "Devices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PunchEntriesHistories_JobSteps",
                        column: x => x.JobStepId,
                        principalSchema: "timeclock",
                        principalTable: "JobSteps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PunchEntriesHistories_JobTypes",
                        column: x => x.JobTypeId,
                        principalSchema: "timeclock",
                        principalTable: "JobTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PunchEntriesHistories_PunchEntries",
                        column: x => x.PunchEntryId,
                        principalSchema: "timeclock",
                        principalTable: "PunchEntries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PunchEntriesHistories_Users",
                        column: x => x.ActionById,
                        principalSchema: "timeclock",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PunchEntriesCurrentStates",
                schema: "timeclock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PunchEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PunchEntriesHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StablePunchEntriesHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PunchEntriesCurrentStates", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_PunchEntriesCurrentStates_RowId", x => x.RowId)
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_PunchEntriesCurrentStates_PunchEntries",
                        column: x => x.PunchEntryId,
                        principalSchema: "timeclock",
                        principalTable: "PunchEntries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PunchEntriesCurrentStates_PunchEntriesHistories",
                        column: x => x.PunchEntriesHistoryId,
                        principalSchema: "timeclock",
                        principalTable: "PunchEntriesHistories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PunchEntriesCurrentStates_PunchEntriesHistories_Stable",
                        column: x => x.StablePunchEntriesHistoryId,
                        principalSchema: "timeclock",
                        principalTable: "PunchEntriesHistories",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                schema: "timeclock",
                table: "AuthorizationClaims",
                columns: new[] { "Id", "RowId", "Type", "Value" },
                values: new object[,]
                {
                    { new Guid("46e16cb7-0939-457f-832e-b39c25ffd196"), 4, "CanConfigureApp", "Can Configure App" },
                    { new Guid("4e43c124-2e61-40bd-a8c2-222c816a519e"), 2, "CanViewOthersPunches", "Can View Others Punches" },
                    { new Guid("570a8132-f550-4bf9-b757-9192f0ff3a49"), 3, "CanEditOthersPunches", "Can Edit Others Punches" },
                    { new Guid("59ddf3ba-7f8a-4ff9-b7dc-cab50abb0c99"), 1, "CanSelectEquipment", "Can Select Equipment" },
                    { new Guid("8bc49e7a-ba30-4bf9-8a52-f73b69da7ae0"), 5, "CanCreateEmployee", "Can Create Employee" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Barcodes_DeactivatedById",
                schema: "timeclock",
                table: "Barcodes",
                column: "DeactivatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Barcodes_UserId",
                schema: "timeclock",
                table: "Barcodes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments",
                schema: "timeclock",
                table: "Departments",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentsToLocation",
                schema: "timeclock",
                table: "DepartmentsToLocations",
                columns: new[] { "DepartmentId", "LocationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentsToLocations_LocationId",
                schema: "timeclock",
                table: "DepartmentsToLocations",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices",
                schema: "timeclock",
                table: "Devices",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_ConfiguredById",
                schema: "timeclock",
                table: "Devices",
                column: "ConfiguredById");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DepartmentsToLocationsId",
                schema: "timeclock",
                table: "Devices",
                column: "DepartmentsToLocationsId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments",
                schema: "timeclock",
                table: "Equipments",
                columns: new[] { "Sku", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_EquipmentTypeId",
                schema: "timeclock",
                table: "Equipments",
                column: "EquipmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentsToDepartmentLocations_DepartmentsToLocationId",
                schema: "timeclock",
                table: "EquipmentsToDepartmentLocations",
                column: "DepartmentsToLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentsToDepartmentLocations_EquipmentId",
                schema: "timeclock",
                table: "EquipmentsToDepartmentLocations",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentsToDepartmentLocations_LinkedById",
                schema: "timeclock",
                table: "EquipmentsToDepartmentLocations",
                column: "LinkedById");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentsToUsers_EquipmentId",
                schema: "timeclock",
                table: "EquipmentsToUsers",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentsToUsers_JobStepId",
                schema: "timeclock",
                table: "EquipmentsToUsers",
                column: "JobStepId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentsToUsers_JobTypeId",
                schema: "timeclock",
                table: "EquipmentsToUsers",
                column: "JobTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentsToUsers_LinkedById",
                schema: "timeclock",
                table: "EquipmentsToUsers",
                column: "LinkedById");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentsToUsers_UnlinkedById",
                schema: "timeclock",
                table: "EquipmentsToUsers",
                column: "UnlinkedById");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentsToUsers_UserId",
                schema: "timeclock",
                table: "EquipmentsToUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentsToUsers_WorkPeriodId",
                schema: "timeclock",
                table: "EquipmentsToUsers",
                column: "WorkPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTypes",
                schema: "timeclock",
                table: "EquipmentTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventAudits",
                schema: "timeclock",
                table: "EventAudits",
                columns: new[] { "EventDateOnly", "EntityType" });

            migrationBuilder.CreateIndex(
                name: "IX_EventAudits_DateOnly",
                schema: "timeclock",
                table: "EventAudits",
                column: "EventDateOnly");

            migrationBuilder.CreateIndex(
                name: "IX_JobTypeStepToEquipments_EquipmentId",
                schema: "timeclock",
                table: "JobTypeStepToEquipments",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTypeStepToEquipments_JobStepId",
                schema: "timeclock",
                table: "JobTypeStepToEquipments",
                column: "JobStepId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTypeStepToEquipments_JobTypeId",
                schema: "timeclock",
                table: "JobTypeStepToEquipments",
                column: "JobTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTypeStepToUsers_JobStepId",
                schema: "timeclock",
                table: "JobTypeStepToUsers",
                column: "JobStepId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTypeStepToUsers_JobTypeId",
                schema: "timeclock",
                table: "JobTypeStepToUsers",
                column: "JobTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTypeStepToUsers_UserId_JobTypeId_JobStepId",
                schema: "timeclock",
                table: "JobTypeStepToUsers",
                columns: new[] { "UserId", "JobTypeId", "JobStepId" },
                unique: true,
                filter: "[JobTypeId] IS NOT NULL AND [JobStepId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Locations",
                schema: "timeclock",
                table: "Locations",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PunchEntries_DeviceId",
                schema: "timeclock",
                table: "PunchEntries",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_PunchEntries_Id",
                schema: "timeclock",
                table: "PunchEntries",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PunchEntries_UserId",
                schema: "timeclock",
                table: "PunchEntries",
                column: "UserId")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_PunchEntries_WorkPeriodId",
                schema: "timeclock",
                table: "PunchEntries",
                column: "WorkPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_PunchEntriesCurrentStates_PunchEntriesHistoryId",
                schema: "timeclock",
                table: "PunchEntriesCurrentStates",
                column: "PunchEntriesHistoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PunchEntriesCurrentStates_PunchEntryId",
                schema: "timeclock",
                table: "PunchEntriesCurrentStates",
                column: "PunchEntryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PunchEntriesCurrentStates_StablePunchEntriesHistoryId",
                schema: "timeclock",
                table: "PunchEntriesCurrentStates",
                column: "StablePunchEntriesHistoryId",
                unique: true,
                filter: "[StablePunchEntriesHistoryId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IC_PunchEntriesHistory_IndexedDate",
                schema: "timeclock",
                table: "PunchEntriesHistories",
                column: "IndexedDate")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_PunchEntriesHistories_ActionById",
                schema: "timeclock",
                table: "PunchEntriesHistories",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_PunchEntriesHistories_DeviceId",
                schema: "timeclock",
                table: "PunchEntriesHistories",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_PunchEntriesHistories_JobStepId",
                schema: "timeclock",
                table: "PunchEntriesHistories",
                column: "JobStepId");

            migrationBuilder.CreateIndex(
                name: "IX_PunchEntriesHistories_JobTypeId",
                schema: "timeclock",
                table: "PunchEntriesHistories",
                column: "JobTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PunchEntriesHistories_PunchEntryId",
                schema: "timeclock",
                table: "PunchEntriesHistories",
                column: "PunchEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_UnregisteredDevices_Name",
                schema: "timeclock",
                table: "UnregisteredDevices",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_AuthorizationClaimId",
                schema: "timeclock",
                table: "UserClaims",
                column: "AuthorizationClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                schema: "timeclock",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId_AuthorizationClaimId",
                schema: "timeclock",
                table: "UserClaims",
                columns: new[] { "UserId", "AuthorizationClaimId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users",
                schema: "timeclock",
                table: "Users",
                column: "EmployeeNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_DefaultJobStepId",
                schema: "timeclock",
                table: "Users",
                column: "DefaultJobStepId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DefaultJobTypeId",
                schema: "timeclock",
                table: "Users",
                column: "DefaultJobTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DepartmentsToLocationId",
                schema: "timeclock",
                table: "Users",
                column: "DepartmentsToLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_JdeId",
                schema: "timeclock",
                table: "Users",
                column: "JdeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_SupervisorJdeId",
                schema: "timeclock",
                table: "Users",
                column: "SupervisorJdeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                schema: "timeclock",
                table: "Users",
                column: "UserName",
                unique: true,
                filter: "[UserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IC_WorkPeriods_WorkDate",
                schema: "timeclock",
                table: "WorkPeriods",
                column: "WorkDate")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkPeriods_UserId",
                schema: "timeclock",
                table: "WorkPeriods",
                column: "UserId")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_WorkPeriodsAudits_UserId",
                schema: "timeclock",
                table: "WorkPeriodsAudits",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPeriodStatusHistories_WorkPeriodId",
                schema: "timeclock",
                table: "WorkPeriodStatusHistories",
                column: "WorkPeriodId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Barcodes",
                schema: "timeclock");

            migrationBuilder.DropTable(
                name: "EquipmentsToDepartmentLocations",
                schema: "timeclock");

            migrationBuilder.DropTable(
                name: "EquipmentsToUsers",
                schema: "timeclock");

            migrationBuilder.DropTable(
                name: "EventAudits",
                schema: "timeclock");

            migrationBuilder.DropTable(
                name: "JobTypeStepToEquipments",
                schema: "timeclock");

            migrationBuilder.DropTable(
                name: "JobTypeStepToUsers",
                schema: "timeclock");

            migrationBuilder.DropTable(
                name: "PunchEntriesCurrentStates",
                schema: "timeclock");

            migrationBuilder.DropTable(
                name: "SentEmails",
                schema: "timeclock");

            migrationBuilder.DropTable(
                name: "UnregisteredDevices",
                schema: "timeclock");

            migrationBuilder.DropTable(
                name: "UserClaims",
                schema: "timeclock");

            migrationBuilder.DropTable(
                name: "WorkPeriodsAudits",
                schema: "timeclock");

            migrationBuilder.DropTable(
                name: "WorkPeriodStatusHistories",
                schema: "timeclock");

            migrationBuilder.DropTable(
                name: "Equipments",
                schema: "timeclock");

            migrationBuilder.DropTable(
                name: "PunchEntriesHistories",
                schema: "timeclock");

            migrationBuilder.DropTable(
                name: "AuthorizationClaims",
                schema: "timeclock");

            migrationBuilder.DropTable(
                name: "EquipmentTypes",
                schema: "timeclock");

            migrationBuilder.DropTable(
                name: "PunchEntries",
                schema: "timeclock");

            migrationBuilder.DropTable(
                name: "Devices",
                schema: "timeclock");

            migrationBuilder.DropTable(
                name: "WorkPeriods",
                schema: "timeclock");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "timeclock");

            migrationBuilder.DropTable(
                name: "DepartmentsToLocations",
                schema: "timeclock");

            migrationBuilder.DropTable(
                name: "JobSteps",
                schema: "timeclock");

            migrationBuilder.DropTable(
                name: "JobTypes",
                schema: "timeclock");

            migrationBuilder.DropTable(
                name: "Departments",
                schema: "timeclock");

            migrationBuilder.DropTable(
                name: "Locations",
                schema: "timeclock");
        }
    }
}
