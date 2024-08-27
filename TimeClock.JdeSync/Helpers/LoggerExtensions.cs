using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Text.Json;
using TimeClock.Data.Models;
using TimeClock.JdeSync.Services;
using Location = TimeClock.Data.Models.Location;
using TimeClock.Data;

namespace TimeClock.JdeSync.Helpers;
internal static class LoggerExtensions
{
    /// <summary>
    /// Logs a new line Information level
    /// </summary>
    /// <param name="logger"></param>
    public static void LogNewLine(this ILogger logger)
    {
        logger.Information(Environment.NewLine);
    }
    /// <summary>
    /// Logs all Entity summaries in <see cref="TcEntityLoader"/> Information Level
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="tcLoader"></param>
    public static void LogChangeSummaries(this ILogger logger, TcEntityLoader tcLoader)
    {
        logger.LogSummary<EquipmentType>(tcLoader.Context);
        logger.LogSummary<Equipment>(tcLoader.Context);
        logger.LogSummary<Department>(tcLoader.Context);
        logger.LogSummary<Location>(tcLoader.Context);
        logger.LogSummary<DepartmentsToLocation>(tcLoader.Context);
        logger.LogSummary<User>(tcLoader.Context);
    }
    /// <summary>
    /// Logs at Information Level
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="logger"></param>
    /// <param name="action"></param>
    /// <param name="jdeId"></param>
    /// <param name="tcId"></param>
    public static void LogInfo<TEntity>(this ILogger logger, string action, object jdeId, Guid tcId)
    {
        logger.Information("{syncProcess} {entityType} {jdeId} {tcId}", action, typeof(TEntity).Name, jdeId, tcId);
    }
    /// <summary>
    /// Logs Information level en entry starting with the phrase "creating new"
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="logger"></param>
    /// <param name="jdeId"></param>
    public static void LogNewItem<TEntity>(this ILogger logger, object jdeId)
    {
        logger.LogInfo<TEntity>("creating new", jdeId, Guid.Empty);
    }
    /// <summary>
    /// Logs Information level en entry starting with the phrase "updating"
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="logger"></param>
    /// <param name="jdeId"></param>
    /// <param name="tcId"></param>
    public static void LogUpdateItem<TEntity>(this ILogger logger, object jdeId, Guid tcId)
    {
        logger.LogInfo<TEntity>("updating", jdeId, tcId);
    }
    /// <summary>
    /// Logs Information level en entry starting with the phrase "inactive"
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="logger"></param>
    /// <param name="jdeId"></param>
    /// <param name="tcId"></param>
    public static void LogDeleteItem<TEntity>(this ILogger logger, object jdeId, Guid tcId)
    {
        logger.LogInfo<TEntity>("inactive", jdeId, tcId);
    }
    /// <summary>
    /// Logs at Information level count of changes detected in context change tracker for an Entity. Includes Modified, Added and Deleted changes.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="logger"></param>
    /// <param name="tcLoader"></param>
    public static void LogFinished<TEntity>(this ILogger logger, TcEntityLoader tcLoader) where TEntity : class
    {
        tcLoader.Context.ChangeTracker.DetectChanges();
        logger.Information("pending {type} changes: {count}", typeof(TEntity).Name, tcLoader.Context.ChangeTracker.Entries<TEntity>()
            .Count(t => t.State == EntityState.Modified || t.State == EntityState.Added || t.State == EntityState.Deleted));
    }
    /// <summary>
    /// Logs at Error level, serializing the entity object provided
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="ex"></param>
    /// <param name="obj"></param>
    /// <param name="syncProcess"></param>
    /// <param name="method"></param>
    public static void LogSyncError(this ILogger logger, Exception ex, object? obj, string syncProcess, [CallerMemberName] string method = "")
    {
        logger.Error(ex, "entity: {entity} | process: {syncProcess} | method: {method}", obj is null ? string.Empty : LoggerExtensions.SafeSerialize(obj, logger), syncProcess, method);
    }
    /// <summary>
    /// Logs a detailed summary at Warning level of changes pending in context change tracker. 
    /// Summary includes properties modified, Id and JdeId properties. Changes detected are Modified, Added and Deleted. 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="logger"></param>
    /// <param name="tcContext"></param>
    public static void LogSummary<TEntity>(this ILogger logger, TimeClockContext tcContext) where TEntity : class
    {
        tcContext.ChangeTracker.DetectChanges();
        var changes = tcContext.ChangeTracker.Entries<TEntity>().Where(t => t.State == EntityState.Modified || t.State == EntityState.Added || t.State == EntityState.Deleted);
        string[] alwaysProps = ["JdeId", "Id"];
        logger.Warning("pending {type} changes: {count}", typeof(TEntity).Name, changes.Count());

        foreach (var entry in changes)
        {
            var modified = entry.State ==
                EntityState.Modified ? entry.Properties.Where(p => p.IsModified || alwaysProps.Contains(p.Metadata.Name))
                    .Select(p => new { prop = p.Metadata.Name, changes = new { before = p.OriginalValue, after = p.CurrentValue } }) :
                ((entry.State == EntityState.Added) ? entry.Properties
                    .Select(p => new { prop = p.Metadata.Name, changes = new { before = (object?)null, after = p.CurrentValue } }) :
                entry.Properties
                    .Select(p => new { prop = p.Metadata.Name, changes = new { before = p.OriginalValue, after = (object?)null } }));
            logger.Warning("{type} was {entityState}: {changes}{NewLine}", typeof(TEntity).Name, entry.State
                , LoggerExtensions.SafeSerialize(modified, logger), Environment.NewLine);
        }
    }
    /// <summary>
    /// Logs at Warning level with the current DateTimeOffset with the message "App started"
    /// </summary>
    /// <param name="logger"></param>
    public static void LogStart(this ILogger logger)
    {
        DateTimeOffset date = DateTimeOffset.Now;
        logger.Warning("App started {startDate:o}", date);
    }
    /// <summary>
    /// Logs at Warning level with the current DateTimeOffset with the message "App finished"
    /// </summary>
    /// <param name="logger"></param>
    public static void LogEnd(this ILogger logger)
    {
        DateTimeOffset date = DateTimeOffset.Now;
        logger.Warning("App finished {startDate:o}", date);
    }

    private static string SafeSerialize(object? obj, ILogger logger)
    {
        if (obj is null) return "[null object]";

        try
        {
            JsonSerializerOptions options = new() { ReferenceHandler = ReferenceHandler.Preserve };
            return JsonSerializer.Serialize(obj);
        }
        catch(Exception ex)
        {
            logger.Error(ex, "error serializing object");
            return $"[error serializing object]";
        }
    }
}
