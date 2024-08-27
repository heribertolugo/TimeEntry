using Microsoft.EntityFrameworkCore;
using Serilog;
using TimeClock.Core;
using TimeClock.Core.Helpers;
using TimeClock.Core.Models;
using TimeClock.Data;
using TimeClock.Data.Models;
using TimeClock.Data.Repository;
using TimeClock.JdeSync.Helpers;

namespace TimeClock.JdeSync.Services;
internal class EmailerService : IDisposable
{
    private bool _disposedValue;
    private ILogger Logger { get; set; }
    private string ConnectionString { get; set; }
    private CancellationToken CancellationToken { get; init; }
    private IDataRepositoryFactory RepositoryFactory { get; set; }
    private EmailConnectivity EmailConnectivity { get; set; }
    private string[]? Bcc { get; set; }
    private string FallbackEmail { get; set; }

    public EmailerService(string connectionString, EmailConnectivity emailConnectivity, ILogger logger, string fallbackEmail, string[]? bcc = null, CancellationToken cancellationToken = default)
    {
        this.ConnectionString = connectionString;
        this.CancellationToken = cancellationToken;
        this.Logger = logger;
        this.RepositoryFactory = new DataRepositoryFactory(connectionString);
        this.EmailConnectivity = emailConnectivity;
        this.FallbackEmail = fallbackEmail;
        this.Bcc = bcc;
    }

    public async Task Start()
    {
        ISentEmailsRepository sentEmailsRepository = this.RepositoryFactory.GetSentEmailsRepository();

        foreach(var userWorkPeriodGroup in (await EmailerService.GetWorkPeriodsMissingPunch(this.RepositoryFactory)).GroupBy(w => w.UserId))
        {
            List<DateOnly> dates = userWorkPeriodGroup.Select(w => w.WorkDate).ToList();
            User? user = userWorkPeriodGroup.FirstOrDefault()?.User;
            string? supervisorEmail = user?.Supervisor?.PrimaryEmail;
            string userFullName = $"{user?.FirstName} {user?.LastName}";
            MissedPunchEmailData emailData = new(userFullName, dates);

            if (this.CancellationToken.IsCancellationRequested)
                this.CancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(user?.Supervisor?.PrimaryEmail ?? this.FallbackEmail))
            {
                this.Logger.Error("Could not get user or email. UserId: {userId}, supervisor: {supervisorEmail}", user?.Id ?? default, supervisorEmail);
#warning send error email to IT
                return;
            }

            supervisorEmail ??= this.FallbackEmail;

            try
            {
                EmailContent emailContent = new(supervisorEmail, emailData);
                bool isFirstTimeEmail = !(await sentEmailsRepository.Get(s => s.Signature == emailContent.Hash).AnyAsync(this.CancellationToken));
                var email = await Emailer.SendMissedPunch(supervisorEmail, emailData, isFirstTimeEmail, this.EmailConnectivity, this.Bcc, this.CancellationToken);
                await EmailerService.AddEmailContentToDb(email, DateTime.Now, sentEmailsRepository, this.CancellationToken);
            }catch(Exception ex)
            {
                this.Logger.Error(ex, "Error while SendMail or AddEmailContentToDb");
                return;
            }
        }

        await EmailerService.SaveToDb(this.RepositoryFactory.Context, this.Logger, this.CancellationToken);
    }

    private static Task AddEmailContentToDb(EmailContent content, DateTime sentOn, ISentEmailsRepository context, CancellationToken cancellationToken = default)
    {
        SentEmail sentEmail = new()
        {
            Message = content.Message,
            SentOn = sentOn,
            SentTo = content.To,
            Subject = content.Subject,
            Signature = content.Hash
        };

        return context.AddAsync(sentEmail, false, cancellationToken);
    }

    private static Task<IEnumerable<WorkPeriod>> GetWorkPeriodsMissingPunch(IDataRepositoryFactory context, CancellationToken cancellationToken = default)
    {
        DateOnly payPeriodEnd = DateOnly.FromDateTime(DateTime.Now.Date.NextDayOfWeek(DayOfWeek.Sunday).AddMicroseconds(-1));
        var query = context.GetWorkPeriodsRepository().GetPreviousIfMissingPunchAsync(payPeriodEnd, false, cancellationToken,
            w => w.User.Supervisor);
        return query;
    }

    private static async Task SaveToDb(TimeClockContext context, ILogger logger, CancellationToken cancellationToken = default)
    {
        logger.Information("saving sent emails to db");
        logger.LogSummary<SentEmail>(context);
        logger.Warning("{changes} changes saved", await context.SaveChangesAsync(true, cancellationToken));
    }

    #region IDisposable Implementation
    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposedValue)
        {
            if (disposing)
            {
                //this.Context.Dispose();
                this.RepositoryFactory.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            this._disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~EmailerService()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion IDisposable Implementation
}
