using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;
using TimeClock.Data.Repository;

namespace TimeClock.Data;

internal static class DefaultData
{
    public static void SeedAll(this ModelBuilder modelBuilder)
    {
        SeedAuthorizationClaims(modelBuilder);
        //MapGetPunchStatusFunction(modelBuilder);
        //MapGetStablePunchStatusFunction(modelBuilder);
    }

    public static AuthorizationClaim[] SeedAuthorizationClaims(this ModelBuilder modelBuilder)
    {
        List<AuthorizationClaim> claims = new();

        foreach (var def in AuthorizationClaimsDefinition.Definitions)
            claims.Add(new AuthorizationClaim() { Type = def.Type, Value = def.Value, Id = def.Id, RowId = def.RowId });

        modelBuilder.Entity<AuthorizationClaim>().HasData(claims);

        return claims.ToArray();
    }

    public static void SeedDbFunctions(this MigrationBuilder migrationBuilder)
    {
        foreach(var function in Functions)
        {
            migrationBuilder.Sql(function.Sql);
        }
    }

    public static void SeedDbFunctions(this MigrationBuilder migrationBuilder, string functionName)
    {
        if (Functions.Any(f => f.Name == functionName))
            migrationBuilder.Sql(Functions.First(f => f.Name == functionName).Sql);
    }

    public static void MapGetPunchStatusFunction(this ModelBuilder modelBuilder)
    {        
        modelBuilder.HasDbFunction(typeof(TimeClockContext).GetMethod(nameof(TimeClockContext.GetPunchStatus), new[] { typeof(Guid) })!)
            .HasName("GetPunchStatus")
            .HasSchema(CommonValues.Schema);
    }

    public static void MapGetStablePunchStatusFunction(this ModelBuilder modelBuilder)
    {
        modelBuilder.HasDbFunction(typeof(TimeClockContext).GetMethod(nameof(TimeClockContext.GetStablePunchStatus), new[] { typeof(Guid) })!)
            .HasName("GetStablePunchStatus")
            .HasSchema(CommonValues.Schema);
    }

    public static void MapIsPreviousMissingPunchFunction(this ModelBuilder modelBuilder)
    {
        modelBuilder.HasDbFunction(typeof(TimeClockContext).GetMethod(nameof(TimeClockContext.IsPreviousMissingPunch), new[] { typeof(Guid) })!)
            .HasName("IsPreviousMissingPunch")
            .HasSchema(CommonValues.Schema);
    }

    public static void MapWorkPeriodJobTypeStepActiveSinceFunction(this ModelBuilder modelBuilder)
    {
        modelBuilder.HasDbFunction(typeof(TimeClockContext).GetMethod(nameof(TimeClockContext.WorkPeriodJobTypeStepActiveSince), new[] { typeof(Guid), typeof(Guid), typeof(DateTime) })!)
            .HasName("WorkPeriodJobTypeStepActiveSince")
            .HasSchema(CommonValues.Schema);
    }

    private static readonly DbFunctionDefinitions[] Functions =[
        new(
@"
-- @id is the ID for a PunchEntriesCurrentStates
CREATE FUNCTION [timeclock].[GetPunchStatus](@id uniqueidentifier)
RETURNS nvarchar(5)
AS
BEGIN
    RETURN (
		SELECT q.[Status] FROM (
			SELECT c.[Id] AS StateId, w.[Id] AS WorkPeriodId, h.[DateTime], IIF(ROW_NUMBER()OVER(PARTITION BY w.[Id] ORDER BY h.[DateTime], h.[RowId] ASC) % 2 <> 0, 'In', 'Out') AS [Status]
			FROM [timeclock].[WorkPeriods] w
			INNER JOIN [timeclock].[PunchEntries]              p1 ON p1.[WorkPeriodId] =  w.[id]
			INNER JOIN [timeclock].[PunchEntriesCurrentStates] c1 ON c1.[PunchEntryId] = p1.[Id] AND c1.[Id] = @id --> filter the WorkPeriod
			LEFT  JOIN [timeclock].[PunchEntries]               p ON  p.[WorkPeriodId] =  w.[Id] --> get the punches for status calculation
			LEFT  JOIN [timeclock].[PunchEntriesCurrentStates]  c ON  c.[PunchEntryId] =  p.[Id]
			LEFT  JOIN [timeclock].[PunchEntriesHistories]      h ON  h.[Id]           =  c.[PunchEntriesHistoryId]
			GROUP BY w.[Id], h.[DateTime], h.[RowId], c.[Id]
		) q
		WHERE q.[StateId] = @id
);
END
", "GetPunchStatus", typeof(string))
        , new(
@"
-- @id is the ID for a PunchEntriesCurrentStates
CREATE FUNCTION [timeclock].[GetStablePunchStatus](@id uniqueidentifier)
RETURNS nvarchar(5)
AS
BEGIN
    RETURN (
		SELECT q.[Status] FROM (
			SELECT c.[Id] AS StateId, w.[Id] AS WorkPeriodId, h.[DateTime], IIF(ROW_NUMBER()OVER(PARTITION BY w.[Id] ORDER BY h.[DateTime], h.[RowId] ASC) % 2 <> 0, 'In', 'Out') AS [Status]
			FROM [timeclock].[WorkPeriods] w
			INNER JOIN [timeclock].[PunchEntries]              p1 ON p1.[WorkPeriodId] =  w.[id]
			INNER JOIN [timeclock].[PunchEntriesCurrentStates] c1 ON c1.[PunchEntryId] = p1.[Id] AND c1.[Id] = @id AND c1.[StablePunchEntriesHistoryId] IS NOT NULL --> filter the WorkPeriod
			LEFT  JOIN [timeclock].[PunchEntries]               p ON  p.[WorkPeriodId] =  w.[Id] --> get the punches for status calculation
			LEFT  JOIN [timeclock].[PunchEntriesCurrentStates]  c ON  c.[PunchEntryId] =  p.[Id]
			LEFT  JOIN [timeclock].[PunchEntriesHistories]      h ON  h.[Id]           =  c.[StablePunchEntriesHistoryId]
			WHERE h.[Id] IS NOT NULL
			GROUP BY w.[Id], h.[DateTime], h.[RowId], c.[Id]
		) q
		WHERE q.[StateId] = @id
);
END
", "GetStablePunchStatus", typeof(string))
        , new(
@"
-- @id is the ID for a WorkPeriods
CREATE FUNCTION [timeclock].[IsPreviousMissingPunch](@id uniqueidentifier)
RETURNS BIT
AS
BEGIN
    RETURN (
            SELECT TOP 1 IIF(COUNT(p.Id) % 2 = 0, 0, 1)
            FROM [timeclock].[WorkPeriods] w
            INNER JOIN [timeclock].[PunchEntries] p ON p.[WorkPeriodId] = w.Id
            INNER JOIN [timeclock].[PunchEntriesCurrentStates] c ON c.[PunchEntryId] = p.Id
            WHERE
                  c.[StablePunchEntriesHistoryId] IS NOT NULL
                  AND w.[RowId] <
                  (
                        SELECT w1.[RowId]
                        FROM [timeclock].[WorkPeriods] w1
                        WHERE w1.[Id] = @id --> filter the WorkPeriod
							AND w1.UserId = w.UserId
                  )
            GROUP BY w.[Id], w.[RowId]
            ORDER BY w.[RowId] DESC
      );
END
", "IsPreviousMissingPunch", typeof(bool))
        , new(
@"
-- @pid is the ID for a PunchEntry, @pid is the ID for a EquipmentsToUsers, @dt is a fallback
CREATE FUNCTION [timeclock].[WorkPeriodJobTypeStepActiveSince](@pid UNIQUEIDENTIFIER, @eid UNIQUEIDENTIFIER, @dt DATETIME2)
RETURNS DATETIME2
AS
BEGIN
    DECLARE @ResultVar AS DATETIME2;
	IF (@ResultVar IS NULL AND @pid IS NOT NULL)
		SET @ResultVar = (
			SELECT h.[EffectiveDateTime]
			FROM [timeclock].[PunchEntries] p
			INNER JOIN [timeclock].[PunchEntriesCurrentStates] c ON c.[PunchEntryId] = p.[Id]
			INNER JOIN [timeclock].[PunchEntriesHistories] h ON h.[Id] = c.[StablePunchEntriesHistoryId]
			WHERE p.[Id] = @pid AND h.[EffectiveDateTime] IS NOT NULL
		);
	IF (@ResultVar IS NULL AND @eid IS NOT NULL)
		SET @ResultVar = (
			SELECT q.[LinkedOnEffective]
	        FROM [timeclock].[EquipmentsToUsers] q
	        WHERE q.[UnLinkedOn] IS NULL AND q.[Id] = @eid AND q.[LinkedOnEffective] IS NOT NULL
		);
	IF (@ResultVar IS NULL)
		SET @ResultVar = @dt;
	RETURN @ResultVar;
END
", "WorkPeriodJobTypeStepActiveSince", typeof(bool))
];
}

public record DbFunctionDefinitions(string Sql, string Name, Type ReturnType);
