using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using TimeClock.Data.Configurations;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;
using Device = TimeClock.Data.Models.Device;
using Location = TimeClock.Data.Models.Location;

namespace TimeClock.Data;

/// <summary>
/// Allows fetching the connection string from App.config when running EF Core commands from CMD for local testing.
/// </summary>
public class TimeClockContextFactory : IDesignTimeDbContextFactory<TimeClockContext>
{
    public TimeClockContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<TimeClockContext> optionsBuilder = new DbContextOptionsBuilder<TimeClockContext>();

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddXmlFile(TimeClockContext.ConfigFileName)
            .Build();

        if (configuration is null)
            throw new Exception("ConfigurationBuilder could not build");

        string? connectionString;
        Exception? exception = null;

        try
        {
            if (configuration!.Providers.FirstOrDefault()!
                .TryGet($"connectionStrings:add:{TimeClockContext.AdminConnectionStringName}:connectionString", out connectionString))
            {
                optionsBuilder.UseSqlServer(connectionString, TimeClockContext.sqlOption);
                return new TimeClockContext(optionsBuilder.Options);
            }
        }catch(Exception ex)
        {

        }
        
        throw new Exception($"The design-time connection string not found! {exception?.Message}", exception);
    }
}


public partial class TimeClockContext : DbContext
{
    private readonly string _connectionString;
    public static readonly string ConfigFileName = "App.dev.config";
    public static readonly string ConnectionStringName = "timeclocksql";
    public static readonly string AdminConnectionStringName = "timeclocksqlSA";
    public static readonly Action<SqlServerDbContextOptionsBuilder> sqlOption = (x) => x.MigrationsHistoryTable("__EFMigrationsHistory", CommonValues.Schema);
    public TimeClockContext(string connectionString)
    {
#if DEBUG
        if (string.IsNullOrWhiteSpace(connectionString))
            connectionString = Helpers.DevConfigurationFileReader.GetConnectionString(TimeClockContext.AdminConnectionStringName);
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string is empty! Pass into constructor or use local App.dev.config");
        this._connectionString = connectionString;
#else
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string is empty!");
        this._connectionString = connectionString;
#endif
    }

    public TimeClockContext(DbContextOptions<TimeClockContext> options)
        : base(options)
    {
        this._connectionString = string.Empty;
    }

    #region DbSets
    public virtual DbSet<AuthorizationClaim> AuthorizationClaims { get; set; }

    public virtual DbSet<Barcode> Barcodes { get; set; }

    //public virtual DbSet<DefaultOvertimeRule> DefaultOvertimeRules { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<DepartmentsToLocation> DepartmentsToLocations { get; set; }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<Equipment> Equipments { get; set; }

    public virtual DbSet<EquipmentType> EquipmentTypes { get; set; }

    public virtual DbSet<EquipmentsToDepartmentLocation> EquipmentsToLocations { get; set; }

    public virtual DbSet<EquipmentsToUser> EquipmentsToUsers { get; set; }

    public virtual DbSet<EventAudit> EventAudits { get; set; }

    public virtual DbSet<JobStep> JobSteps { get; set; }

    public virtual DbSet<JobType> JobTypes { get; set; }

    //public virtual DbSet<JobTypePayRule> JobTypePayRules { get; set; }

    public virtual DbSet<JobTypeStepToEquipment> JobTypesToEquipments { get; set; }

    public virtual DbSet<JobTypeStepToUser> JobTypeStepsToUsers { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    //public virtual DbSet<PaySettings> PaySettings { get; set; }

    public virtual DbSet<PunchEntriesHistory> PunchEntriesHistories { get; set; }

    public virtual DbSet<PunchEntry> PunchEntries { get; set; }

    public virtual DbSet<PunchEntriesCurrentState> PunchEntriesCurrentStates { get; set; }

    //public virtual DbSet<RateIncrease> RateIncreases { get; set; }

    public virtual DbSet<SentEmail> SentEmails { get; set; }

    //public virtual DbSet<SpecialOvertimeRule> SpecialOvertimeRules { get; set; }

    //public virtual DbSet<SpecialPay> SpecialPays { get; set; }

    public virtual DbSet<UnregisteredDevice> UnregisteredDevices { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserClaim> UserClaims { get; set; }

    public virtual DbSet<WorkPeriod> WorkPeriods { get; set; }

    public virtual DbSet<WorkPeriodsAudit> WorkPeriodsAudits { get; set; }

    public virtual DbSet<WorkPeriodJobTypeStep> WorkPeriodJobTypeSteps { get; set; }

    public virtual DbSet<WorkPeriodStatusHistory> WorkPeriodStatusHistories { get; set; }
    #endregion DbSets

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        
        if (!string.IsNullOrWhiteSpace(this._connectionString))
            optionsBuilder.UseSqlServer(this._connectionString, sqlOption);

#if DEBUG
        //optionsBuilder.LogTo(Console.WriteLine);
#endif
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new BarcodeConfiguration().Configure(modelBuilder.Entity<Barcode>());
        new DepartmentConfiguration().Configure(modelBuilder.Entity<Department>());
        new DepartmentsToLocationConfiguration().Configure(modelBuilder.Entity<DepartmentsToLocation>());
        new DeviceConfiguration().Configure(modelBuilder.Entity<Device>());
        new EquipmentConfiguration().Configure(modelBuilder.Entity<Equipment>());
        new EquipmentTypeConfiguration().Configure(modelBuilder.Entity<EquipmentType>());
        new EquipmentsToDepartmentLocationConfiguration().Configure(modelBuilder.Entity<EquipmentsToDepartmentLocation>());
        new EquipmentsToUserConfiguration().Configure(modelBuilder.Entity<EquipmentsToUser>());
        new LocationConfiguration().Configure(modelBuilder.Entity<Location>());
        new PunchEntriesHistoryConfiguration().Configure(modelBuilder.Entity<PunchEntriesHistory>());
        new PunchEntryConfiguration().Configure(modelBuilder.Entity<PunchEntry>());
        new UserConfiguration().Configure(modelBuilder.Entity<User>());
        new PunchEntryCurrentStateConfiguration().Configure(modelBuilder.Entity<PunchEntriesCurrentState>());
        new EventAuditConfiguration().Configure(modelBuilder.Entity<EventAudit>());
        new UnregisteredDevicesConfiguration().Configure(modelBuilder.Entity<UnregisteredDevice>());
        new AuthorizationClaimConfiguration().Configure(modelBuilder.Entity<AuthorizationClaim>());
        new UserClaimsConfiguration().Configure(modelBuilder.Entity<UserClaim>());
        new WorkPeriodConfiguration().Configure(modelBuilder.Entity<WorkPeriod>());
        new WorkPeriodsAuditsConfiguration().Configure(modelBuilder.Entity<WorkPeriodsAudit>());
        new WorkPeriodStatusHistoryConfiguration().Configure(modelBuilder.Entity<WorkPeriodStatusHistory>());
        new JobTypeConfiguration().Configure(modelBuilder.Entity<JobType>());
        new JobStepConfiguration().Configure(modelBuilder.Entity<JobStep>());
        //new PaySettingsConfiguration().Configure(modelBuilder.Entity<PaySettings>());
        //new RateIncreaseConfiguration().Configure(modelBuilder.Entity<RateIncrease>());
        //new SpecialPayConfiguration().Configure(modelBuilder.Entity<SpecialPay>());
        //new JobTypePayRuleConfiguration().Configure(modelBuilder.Entity<JobTypePayRule>());
        //new SpecialOvertimeRuleConfiguration().Configure(modelBuilder.Entity<SpecialOvertimeRule>());
        //new DefaultOvertimeRuleConfiguration().Configure(modelBuilder.Entity<DefaultOvertimeRule>());
        new JobTypeStepToEquipmentConfiguration().Configure(modelBuilder.Entity<JobTypeStepToEquipment>());
        new JobTypeStepToUserConfiguration().Configure(modelBuilder.Entity<JobTypeStepToUser>());
        new WorkPeriodJobTypeStepConfiguration().Configure(modelBuilder.Entity<WorkPeriodJobTypeStep>());
        new SentEmailConfiguration().Configure(modelBuilder.Entity<SentEmail>());

        this.OnModelCreatingPartial(modelBuilder);

        DefaultData.SeedAll(modelBuilder);

#if DEBUG
        //SampleData.SeedAll(modelBuilder);
#endif 

        //modelBuilder.SeedFunctions();
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    /// <summary>
    /// Get the In/Out PunchEntry status for the newest PunchEntriesHistory
    /// </summary>
    /// <param name="id">The ID of the <see cref="PunchEntriesCurrentState"/></param>
    /// <returns></returns>
    public IQueryable<string?> GetPunchStatus(Guid id) => this.FromExpression(() => this.GetPunchStatus(id));
    /// <summary>
    /// Get the In/Out PunchEntry status for the newest stable PunchEntriesHistory
    /// </summary>
    /// <param name="id">The ID of the <see cref="PunchEntriesCurrentState"/></param>
    /// <returns></returns>
    public IQueryable<string?> GetStablePunchStatus(Guid id) => this.FromExpression(() => this.GetStablePunchStatus(id));
    /// <summary>
    /// Returns if the immediate previous WorkPeriod for the WorkPeriodId passed in, is missing a PunchEntry which was a punch-out. 
    /// This will consider the User of the WorkPeriodId passed in.
    /// </summary>
    /// <param name="id">The ID of a <see cref="WorkPeriod"/> used to get a previous <see cref="WorkPeriod"/></param>
    /// <returns></returns>
    public IQueryable<bool?> IsPreviousMissingPunch(Guid id) => this.FromExpression(() => this.IsPreviousMissingPunch(id));
    /// <summary>
    /// Returns a DateTime which corresponds to the value which to use as the ActiveSince for a <see cref="WorkPeriodJobTypeStep"/>. 
    /// <inheritdoc cref="WorkPeriodJobTypeStep.ActiveSince"/>
    /// </summary>
    /// <param name="punchEntryId"></param>
    /// <param name="equipmentsToUsersId"></param>
    /// <param name="fallback"></param>
    /// <returns></returns>
    public IQueryable<DateTime?> WorkPeriodJobTypeStepActiveSince(Guid? punchEntryId, Guid? equipmentsToUsersId, DateTime fallback) 
        => this.FromExpression(() => this.WorkPeriodJobTypeStepActiveSince(punchEntryId, equipmentsToUsersId, fallback));
}
