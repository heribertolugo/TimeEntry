using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Collections.Frozen;
using TimeClock.Data;
using TimeClock.Data.Models.Jde;
using TimeClock.JdeSync.Helpers;

namespace TimeClock.JdeSync.Services;
internal class JdeEntityLoader : IDisposable
{
    private CancellationToken CancellationToken { get; init; }
    private string ConnectionString { get; init; }
    private ILogger Logger { get; init; }
    private bool _disposedValue;


    public JdeEntityLoader(string connectionString, ILogger logger, CancellationToken cancellationToken)
    {
        this.ConnectionString = connectionString;
        this.CancellationToken = cancellationToken;
        this.Logger = logger;
        this.Context = new JdeContext(connectionString);
    }

    public async Task Load()
    {
        await this.LoadJdeEquipmentLocations(); // loads JdeEquipmentLocations and JdeEquipment
        this.Logger.LogNewLine();

        await this.LoadJdeEquipmentTypes(); // loads JdeEquipmentTypes
        this.Logger.LogNewLine();

        await this.LoadJdeLocations();
        this.Logger.LogNewLine();

        //LoadJdeEquipmentWithTypes();
        await this.LoadJdeEmployees(); // loads JdeEmployees and JdeLocations
        this.Logger.LogNewLine();

        await this.LoadJdeJobTypes();
        this.Logger.LogNewLine();

        await this.LoadJdeJobSteps();
        this.Logger.LogNewLine();

        await this.LoadUsersJobTypeSteps();
        this.Logger.LogNewLine();
    }


    public JdeContext Context { get; private set; }
    public IEnumerable<IJdeEntityModel> EquipmentTypes { get; private set; } = Enumerable.Empty<JdeCustomCode>();
    public IEnumerable<IJdeEntityModel> Equipment { get; private set; } = Enumerable.Empty<JdeEquipment>();
    public IEnumerable<IJdeEntityModel> EquipmentToLocations { get; private set; } = Enumerable.Empty<JdeEquipmentLocation>();
    public IEnumerable<IJdeEntityModel> Locations { get; private set; } = Enumerable.Empty<JdeLocation>();
    public IEnumerable<IJdeEntityModel> Employees { get; private set; } = Enumerable.Empty<JdeEmployee>();
    public IEnumerable<IJdeEntityModel> JobTypes { get; private set; } = Enumerable.Empty<JdeEmployee>();
    public IEnumerable<IJdeEntityModel> JobSteps { get; private set; } = Enumerable.Empty<JdeEmployee>();

    public FrozenDictionary<int, IEnumerable<UserJobTypeStep>> UserJobTypeSteps {  get; private set; }

    public static FrozenSet<char> ActiveEmployeeStatuses = Array.Empty<char>().ToFrozenSet();

    public bool FilterEmployeesToActive { get; set; } = false;

    public async Task LoadJdeEquipmentTypes()
    {
        if (this.CancellationToken.IsCancellationRequested) return;
        this.Logger.Information(nameof(LoadJdeEquipmentTypes));
        var query = this.Context.JdeCustomCodes.AsNoTracking()
            .Where(c => c.Codes == "C2" && c.ProductCode == "12")
            .OrderBy(c => c.Code); // do not trim c.Code - order is important
        this.EquipmentTypes = await query.ToListAsync();
    }

    ///// <summary>
    ///// not used. same functionality is included in LoadJdeEquipmentLocations() and LoadJdeEquipmentTypes()
    ///// </summary>
    //public async Task LoadJdeEquipmentWithTypes()
    //{
    //    if (this._cancellationToken.IsCancellationRequested) return;
    //    this._logger.Information(nameof(LoadJdeEquipmentWithTypes));
    //    var query = this.Context.JdeEquipments.AsNoTracking()
    //        .Join(this.Context.JdeCustomCodes.AsNoTracking()
    //        , a => a.FK.Trim()
    //        , b => b.Code.Trim()
    //        , (a, b) => new JdeEquipment
    //        {
    //            Description1 = a.Description1,
    //            Description2 = a.Description2,
    //            Description3 = a.Description3,
    //            EquipmentNumber = a.EquipmentNumber,
    //            FK = a.FK,
    //            Id = a.Id,
    //            Status = a.Status,
    //            EquipmentType = b
    //        }
    //        )
    //        .Where(q => q.Status == "AV" && q.EquipmentType.ProductCode == "12" && q.EquipmentType.Codes == "C2")
    //        .OrderBy(q => q.Id);

    //    this.Equipment = await query.ToListAsync();
    //}

    /// <summary>
    /// Loads JdeEquipmentLocations and JdeEquipment 
    /// </summary>
    public async Task LoadJdeEquipmentLocations()
    {
        if (this.CancellationToken.IsCancellationRequested) return;
        this.Logger.Information(nameof(LoadJdeEquipmentLocations));
        var query = this.Context.JdeEquipmentLocations.AsNoTracking()
            .Include(l => l.Location)
            .Include(l => l.Equipment)
            .Where(l => l.CurrentState == 'C' && l.Location.CategoryCodeBusinessUnit17 != "CLS" && l.Location.Company == CommonValues.CompanyId && l.Equipment != null)
            .OrderBy(l => l.Id)
            ;

        this.EquipmentToLocations = await query.ToListAsync();
        //this.Locations = this.EquipmentToLocations.Select(q => ((JdeEquipmentLocation)q).Location)
        //    .DistinctBy(l => new { l.BusinessUnit, l.AddressNumber })
        //    .OrderBy(q => q.BusinessUnit)
        //    .ToList();
        this.Equipment = this.EquipmentToLocations.Select(q => ((JdeEquipmentLocation)q).Equipment)
            .OrderBy(q => q.Id)
            .ToList();
    }

    public async Task LoadJdeLocations()
    {
        if (this.CancellationToken.IsCancellationRequested) return;
        this.Logger.Information(nameof(LoadJdeLocations));
        List<JdeLocation> locations = new List<JdeLocation>(this.Locations.Cast<JdeLocation>());

        var query = this.Context.JdeLocations.AsNoTracking()
            .Where(l => l.Company == CommonValues.CompanyId)
            .OrderBy(l => l.BusinessUnit);

        this.Locations = await query.ToListAsync();
    }
    /// <summary>
    /// Loads all <see cref="JdeEmployee">employees</see> for <see cref="CommonValues.CompanyId">company ID</see>, including their <see cref="JdeLocation">location</see>, <see cref="JdeEmployee">supervisor</see>, <see cref="JdeEmployee">subordinates</see> and <see cref="JdeEmail">primary email</see>
    /// </summary>
    /// <returns></returns>
    public async Task LoadJdeEmployees()
    {
        if (this.CancellationToken.IsCancellationRequested) return;
        this.Logger.Information(nameof(LoadJdeEmployees));
        var query = this.Context.JdeEmployees.AsNoTracking()
            .Include(m => m.Location)
            .Include(m => m.Supervisor)
            .Include(m => m.Subordinates)
            .Include(m => m.Emails.OrderBy(e => e.EmployeeId).Take(1)) //.Where(e => EF.Functions.Like(e.Email, $"%company.com"))
#warning add config to DI or someway to access values to use filtering for email addresses by company domain
            .Where(m => m.Company == CommonValues.CompanyId);

        if (this.FilterEmployeesToActive)
        {
            char[] activeStatuses = JdeEntityLoader.ActiveEmployeeStatuses.ToArray();
            query = query.Where(m => activeStatuses.Contains(m.PayStatus));
        }

        query = query.OrderBy(m => m.Id);

        this.Employees = await query.AsSplitQuery().ToListAsync();
    }

    public async Task LoadJdeJobTypes()
    {
        if (this.CancellationToken.IsCancellationRequested) return;
        this.Logger.Information(nameof(LoadJdeJobTypes));
        var query = this.Context.JdeCustomCodes.AsNoTracking()
            .Where(m => m.ProductCode == "06" && m.Codes == "G")
            .OrderBy(m => m.Code); // do not trim c.Code - order is important

        this.JobTypes = await query.ToListAsync();
    }

    public async Task LoadJdeJobSteps()
    {
        if (this.CancellationToken.IsCancellationRequested) return;
        this.Logger.Information(nameof(LoadJdeJobTypes));
        var query = this.Context.JdeCustomCodes.AsNoTracking()
            .Where(m => m.ProductCode == "06" && m.Codes == "GS")
            .OrderBy(m => m.Code); // do not trim c.Code - order is important

        this.JobSteps = await query.ToListAsync();
    }

    public async Task LoadUsersJobTypeSteps()
    {
        if (this.CancellationToken.IsCancellationRequested) return;
        this.Logger.Information(nameof(LoadUsersJobTypeSteps));
        int filterDate = DateTime.Now.Date.AddDays(-60).ToJdeDate();

        var query = this.Context.JdeEmployeeTimeEntryHistories.AsNoTracking()
            .Where(t => t.CompanyHome.Trim() == CommonValues.CompanyId.Trim() && t.DateWorked > filterDate)
            .Select(t => new { t.EmployeeId, t.JobType, t.JobStep})
            .Distinct()
            .OrderBy(t => t.EmployeeId)
            .ThenBy(t => t.JobType)
            .ThenBy(t => t.JobStep);

        this.UserJobTypeSteps = (await query.ToListAsync())
            .GroupBy(t => t.EmployeeId)
            .Where(g => g.Count() > 1)
            .ToDictionary(g => g.Key,
                g => g.Select(g => new UserJobTypeStep(g.EmployeeId, g.JobType, g.JobStep)))
            .ToFrozenDictionary();
    }

    public async Task LoadEquipmentJobTypeSteps()
    {
        var query = this.Context.JdeEmployeeTimeEntryHistories.AsNoTracking()
            .Where(t => t.CompanyHome.Trim() == CommonValues.CompanyId.Trim() && string.Compare(t.EquipmentId, " ") > 0)
            .Select(t => new { t.EquipmentId, t.JobType, t.JobStep, t.UnionCode })
            .Distinct()
            .OrderBy(t => t.EquipmentId)
            .ThenBy(t => t.JobType)
            .ThenBy(t => t.JobStep)
            .ThenBy(t => t.UnionCode);
    }


    #region IDisposable
    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposedValue)
        {
            if (disposing)
            {
                this.Context.Dispose();
            }

            this._disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion IDisposable

    public record struct UserJobTypeStep(int? EmployeeId, string? JobTypeCode, string? JobStepCode);
}
