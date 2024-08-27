using System.Collections.Concurrent;
using System.Runtime.Versioning;
using TimeClock.Data;
using TimeClock.Data.Models;
using TimeClock.Data.Repository;

namespace TimeClock.Api.Helpers;

public static class LoggerExtensions
{
    public static void LogAudit<T>(this ILogger logger, string message, EventId eventId, Guid entityId, IEventAuditRepository dataRepository, bool success = false) where T : IEntityModel
    {
        EventAudit eventAudit = new()
        {
            Id = Guid.NewGuid(),
            EventId = eventId.Id,
            EventName = eventId.Name ?? string.Empty,
            EventDescription = message[..Math.Min(message.Length,3000)],
            Success = success,
            EntityType = typeof(T).Name,
            EntityId = entityId,
            EventDate = DateTime.UtcNow
        };

        try
        {
            logger.Log(LogLevel.Critical, eventId, (dataRepository, eventAudit), null, (a, e) => string.Empty);
        }catch(Exception ex)
        {
            logger.LogError(ex, "EventAuditLogger crashed");
        }
    }
    public static void LogAudit<T>(this ILogger logger, string message, EventId eventId, Guid entityId, IDataRepositoryFactory dataRepository, bool success = false) where T : IEntityModel
    {
        logger.LogAudit<T>(message, eventId, entityId, dataRepository.GetEventAuditsRepository(), success);
    }
}

public sealed class EventAuditLogger : ILogger
{
    public EventAuditLogger(string name)
    {
        this.Name = name;
    }

    public string Name { get; set; }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

    public bool IsEnabled(LogLevel logLevel) => logLevel == LogLevel.Critical;

    public async void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        (IEventAuditRepository, EventAudit)? data = state as (IEventAuditRepository, EventAudit)?;

        if (data is null || !this.IsEnabled(logLevel))
            return;

        await data.Value.Item1.InsertDirectlyToDbAsync(data.Value.Item2);
    }
}

public sealed class EventAuditLoggerConfiguration
{
    public int EventId { get; set; }

}

[UnsupportedOSPlatform("browser")]
[ProviderAlias("EventAuditLogger")]
public sealed class EventAuditLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, EventAuditLogger> _loggers =
        new(StringComparer.OrdinalIgnoreCase);

    public EventAuditLoggerProvider() { }

    public ILogger CreateLogger(string categoryName)
    {
        return new EventAuditLogger(nameof(EventAuditLogger));// this._loggers.GetOrAdd(categoryName, name => new EventAuditLogger(name));
    }

    public void Dispose()
    {
        this._loggers.Clear();
    }
}
