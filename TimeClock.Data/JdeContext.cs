using Microsoft.EntityFrameworkCore;
using TimeClock.Data.Configurations.Jde;
using TimeClock.Data.Models.Jde;

namespace TimeClock.Data;

public partial class JdeContext : DbContext
{
    private readonly string _connectionString;
    private static readonly string ConnectionStringName = "oraclejde";
    public JdeContext(string? connectionString = null)
    {
#if DEBUG
        if (string.IsNullOrWhiteSpace(connectionString))
            connectionString = Helpers.DevConfigurationFileReader.GetConnectionString(JdeContext.ConnectionStringName);
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string is empty! Pass into constructor or use local App.dev.config");
        this._connectionString = connectionString;
#else
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string is empty!");
        this._connectionString = connectionString;
#endif
    }

    public JdeContext(DbContextOptions<JdeContext> options)
        : base(options)
    {
        this._connectionString = string.Empty;
    }
    /// <summary>
    /// F1201
    /// </summary>
    public virtual DbSet<JdeEquipment> JdeEquipments { get; set; }
    /// <summary>
    /// F1204
    /// </summary>
    public virtual DbSet<JdeEquipmentLocation> JdeEquipmentLocations { get; set; }
    /// <summary>
    /// F060116
    /// </summary>
    public virtual DbSet<JdeEmployee> JdeEmployees { get; set; }
    /// <summary>
    /// F0006
    /// </summary>
    public virtual DbSet<JdeLocation> JdeLocations { get; set; }
    /// <summary>
    /// F06116Z1 | Employee Transactions - Batch File
    /// </summary>
    public virtual DbSet<JdeEmployeeTimeEntryImport> JdeEmployeeTimeEntryImports { get; set; }
    /// <summary>
    /// F06116 | Employee Transaction Detail File
    /// </summary>
    public virtual DbSet<JdeEmployeeTimeEntry> JdeEmployeeTimeEntries { get; set; }
    /// <summary>
    /// F0618 | Employee Transaction History
    /// </summary>
    public virtual DbSet<JdeEmployeeTimeEntryHistory> JdeEmployeeTimeEntryHistories { get; set; }
    /// <summary>
    /// F0005
    /// </summary>
    public virtual DbSet<JdeCustomCode> JdeCustomCodes { get; set; }
    /// <summary>
    /// F0002
    /// </summary>
    public virtual DbSet<JdeNextNumber> JdeNextNumbers { get; set; }
    /// <summary>
    /// F01151
    /// </summary>
    public virtual DbSet<JdeEmail> JdeEmails { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (!string.IsNullOrWhiteSpace(this._connectionString))
            optionsBuilder.UseOracle(this._connectionString);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new JdeEmployeeConfiguration().Configure(modelBuilder.Entity<JdeEmployee>());
        new JdeEquipmentConfiguration().Configure(modelBuilder.Entity<JdeEquipment>());
        new JdeEquipmentLocationConfiguration().Configure(modelBuilder.Entity<JdeEquipmentLocation>());
        new JdeLocationConfiguration().Configure(modelBuilder.Entity<JdeLocation>());
        new JdeEmployeeTimeEntryImportConfiguration().Configure(modelBuilder.Entity<JdeEmployeeTimeEntryImport>());
        new JdeCustomCodeConfiguration().Configure(modelBuilder.Entity<JdeCustomCode>());
        new JdeNextNumbersConfiguration().Configure(modelBuilder.Entity<JdeNextNumber>());
        new JdeEmailConfiguration().Configure(modelBuilder.Entity<JdeEmail>());
        new JdeEmployeeTimeEntryConfiguration().Configure(modelBuilder.Entity<JdeEmployeeTimeEntry>());
        new JdeEmployeeTimeEntryHistoryConfiguration().Configure(modelBuilder.Entity<JdeEmployeeTimeEntryHistory>());

        this.OnModelCreatingPartial(modelBuilder);
    }


    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
