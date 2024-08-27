using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;
using System.Text;
using TimeClock.Core;
using TimeClock.Data;
using TimeClock.Data.Models;
using TimeClock.Data.Models.Jde;
using TimeClock.JdeSync.Helpers;
using Location = TimeClock.Data.Models.Location;

namespace TimeClock.JdeSync.Services;
internal class TcToJdeEtl : IDisposable
{
    #region Private Members/Properties
    private bool _disposedValue;
    private ILogger Logger { get; set; }
    private CancellationToken CancellationToken { get; set; }
    private TimeClockContext TcContext { get; set; }
    private JdeContext JdeContext { get; set; }
    private Location[] Locations { get; set; }

    #endregion Members/Properties


    public TcToJdeEtl(string tcConnectionString, string jdeConnectionString, ILogger logger, CancellationToken cancellationToken)
    {
        this.Logger = logger;
        this.CancellationToken = cancellationToken;
        this.TcContext = new TimeClockContext(tcConnectionString);
        this.JdeContext = new JdeContext(jdeConnectionString);
    }

    public async Task Start()
    {
        this.TcContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(2));
        this.Locations = this.TcContext.Locations.ToArray();
        string outFileName = $"TimeClockToJdeTimeEntries{DateTime.Now:yyyyMMdd_HHmmssfffffff}.txt";
        var query = this.TcContext.WorkPeriods
            .Where(w => w.Purpose != Data.Models.WorkPeriodPurpose.SpecialTime 
                && w.WorkPeriodStatusHistories.OrderByDescending(h => h.DateTime).First().Status == Data.Models.WorkPeriodStatus.Pending)
            .Include(w => w.EquipmentsToUsers.OrderBy(q => q.LinkedOnEffective)).ThenInclude(t => t.Equipment)
            .Include(w => w.EquipmentsToUsers.OrderBy(q => q.LinkedOnEffective)).ThenInclude(t => t.JobStep)
            .Include(w => w.EquipmentsToUsers.OrderBy(q => q.LinkedOnEffective)).ThenInclude(t => t.JobType)
            .Include(w => w.WorkPeriodJobTypeSteps.OrderBy(w => w.ActiveSince)).ThenInclude(j => j.JobType)
            .Include(w => w.WorkPeriodJobTypeSteps.OrderBy(w => w.ActiveSince)).ThenInclude(j => j.JobStep)
            .Include(w => w.PunchEntries.Where(p => p.CurrentState.StablePunchEntriesHistory != null).OrderBy(h => h.CurrentState.StablePunchEntriesHistory.EffectiveDateTime))
                .ThenInclude(p => p.Device.DepartmentsToLocations.Location)
            .Include(w => w.PunchEntries.Where(p => p.CurrentState.StablePunchEntriesHistory != null).OrderBy(h => h.CurrentState.StablePunchEntriesHistory.EffectiveDateTime))
                .ThenInclude(p => p.CurrentState.StablePunchEntriesHistory.JobStep)
            .Include(w => w.PunchEntries.Where(p => p.CurrentState.StablePunchEntriesHistory != null).OrderBy(h => h.CurrentState.StablePunchEntriesHistory.EffectiveDateTime))
                .ThenInclude(p => p.CurrentState.StablePunchEntriesHistory.JobType)
            .OrderBy(w => w.User.RowId)
            .ThenBy(w => w.WorkDate)
            ;
        try
        {
            List<WorkPeriod> result = [];
            int currentPageNumber = 0;
            int itemsPerPage = 50;

            while ((result = query.Skip(currentPageNumber * itemsPerPage).Take(itemsPerPage).AsSplitQuery().ToList()).Count > 0)
            {
                foreach (var payPeriodEntries in result.GroupBy(w => w.PayPeriodEnd))
                {
                    await foreach (var timeEntry in AddTimeEntryBatch(payPeriodEntries, this.JdeContext, this.TcContext, this.Logger, this.CancellationToken))
                    {
                        // don't await. lets continue processing the next batch of entries while the current is written to file
#pragma warning disable CS4014
                        OutputEntryToFile(timeEntry, this.JdeContext, outFileName, this.Logger, this.CancellationToken);
#pragma warning restore CS4014
                    }
                }

                currentPageNumber++;
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private async static IAsyncEnumerable<JdeEmployeeTimeEntryImport> AddTimeEntryBatch(
        IEnumerable<WorkPeriod> payPeriod, JdeContext context, TimeClockContext tcContext, ILogger logger, CancellationToken cancellationToken, Action<JdeEmployeeTimeEntryImport>? afterCreateEntry = null)
    {
        string batchId = DateTime.Now.ToString("yyyyMMdd_HHmm");
        string appId = typeof(TcToJdeEtl).AssemblyQualifiedName!;
        int totalEntriesToProcess = await GetNumberOfTimeEntries(tcContext, payPeriod.MinBy(p => p.WorkDate)!.WorkDate,
            payPeriod.MinBy(p => p.WorkDate)!.WorkDate, logger, cancellationToken);
        int? maxPayrollTransactionNumber = await TcToJdeEtl.GetNextNumber(totalEntriesToProcess, context.Database.GetConnectionString()!);
        int currentDateAsJde = DateTime.Now.ToJdeDate();

        if (!maxPayrollTransactionNumber.HasValue) throw new Exception("Next number is not available");

        JdeTimeEntryCreator entryCreator = new()
        {
            EdiId = appId,
            EdiSubmitDate = currentDateAsJde,
            EdiBatchNumber = batchId,
            BatchDate = currentDateAsJde,
            DateUpdated = currentDateAsJde,
            ProgramId = appId
        };

        // we are loading jde employees so that we can use PayTypeCalculator to determine if a specific time entry record needs to be split into overtime
        // or if an additional record needs to be made for overtime
        // jde employees have their union data which is used for some employees to determine overtime
        // a section needs to be made into appsettings to control over time rules for some unions, some projects and possibly other factors
        foreach (var userWorkPeriods in payPeriod.GroupBy(w => w.User))
        {
            // process a user
            int? employeeId = userWorkPeriods.Key.JdeId;
            User user = userWorkPeriods.Key;
            if (!employeeId.HasValue) continue;
            string? employeeUsername = user.UserName;
            string transactionNumber = employeeId.ToString()!;
            int currentLineNumber = 1;
            double employeeHoursTally = 0;
            DateOnly? currentPayPeriod = null;
            IEnumerator<EquipmentsToUser>? userEquipment = null;

            // process each day of work for current user
            foreach (WorkPeriod? userWorkPeriod in userWorkPeriods)
            {
                entryCreator.DateWorked = userWorkPeriod.WorkDate.ToJdeDate();
                // i hate creating an array from a collection since it causes unneeded early iteration. but we need to be able to access by index
                var indexableJobTypeSteps = userWorkPeriod.WorkPeriodJobTypeSteps.ToArray();
                if (!currentPayPeriod.HasValue || userWorkPeriod.WorkDate > currentPayPeriod.Value)
                {
                    currentPayPeriod = userWorkPeriod.WorkDate.NextDayOfWeek(DayOfWeek.Sunday);
                }

                #region old code with work period job type steps
                if (indexableJobTypeSteps.Length != 0)
                {
                    // if any. if not, use user defaults
                    for (var jobTypeStepIndex = 0; jobTypeStepIndex < indexableJobTypeSteps.Length; jobTypeStepIndex++)
                    {
                        if (userEquipment is null)
                        {
                            userEquipment = userWorkPeriod.EquipmentsToUsers.GetEnumerator();
                            userEquipment.MoveNext();
                        }
                        TimeOnly jobTypeStepTimeBegin = indexableJobTypeSteps[jobTypeStepIndex].ActiveSince.ToTimeOnly();
                        // get the next job type/step to determine when the begin job type/step expires
                        // if none found, use the last punch for the day, or the end of the day for the begin job type/step
                        TimeOnly jobTypeStepTimeEnd = (indexableJobTypeSteps.TryGetAtIndex(jobTypeStepIndex + 1)?.ActiveSince
                            ?? userWorkPeriod.PunchEntries.OrderBy(p => p.CurrentState?.StablePunchEntriesHistory?.DateTime ?? DateTime.MinValue)
                                .LastOrDefault(p => p.CurrentState?.StablePunchEntriesHistory?.EffectiveDateTime != null)?.CurrentState?.StablePunchEntriesHistory?.EffectiveDateTime
                            ?? throw new Exception("Could not determine the end of JobTypeStep time")).ToTimeOnly();

                        double totalHours = jobTypeStepTimeEnd.ToTimeSpan().Subtract(jobTypeStepTimeBegin.ToTimeSpan()).TotalHours;
                        employeeHoursTally += totalHours;
                        bool isOvertime = employeeHoursTally > 40;
                        bool isPreOvertimeProcessed = false;
                        string jobTypeToGl = ""; // await TcToJdeEtl.GetGlCodeForJobType(workperiod.JobType);

                        bool wasEquipmentProcessed = false;
                        TimeOnly? equipmentBegin;
                        TimeOnly? equipmentEnd = null;

                        entryCreator.EdiTransactionNumber = transactionNumber;
                        entryCreator.EmployeeId = employeeId;
                        entryCreator.JobType = indexableJobTypeSteps[jobTypeStepIndex].JobType?.JdeId;
                        entryCreator.JobStep = indexableJobTypeSteps[jobTypeStepIndex].JobStep?.JdeId;
                        entryCreator.UnionCode = user.UnionCode;

                        // process equipment used
                        while (((equipmentBegin = userEquipment.GetCurrent()?.LinkedOnEffective?.ToTimeOnly()) ?? TimeOnly.MinValue) >= jobTypeStepTimeBegin &&
                            (equipmentEnd = userEquipment.GetCurrent()?.UnLinkedOnEffective?.ToTimeOnly() ?? jobTypeStepTimeEnd) <= jobTypeStepTimeEnd)
                        {
                            bool wasEquipmentHandled = false;
                            wasEquipmentProcessed = true;

                            if (isOvertime && !isPreOvertimeProcessed)
                            {
                                entryCreator.EdiLineNumber = currentLineNumber * 100;
                                entryCreator.PayrollTransactionNumber = totalEntriesToProcess++;
                                entryCreator.HoursWorked = (decimal)((totalHours - 40) * 100);
                                entryCreator.DbaCode = 1;
                                entryCreator.GlAccount = $"{userWorkPeriod.User.DepartmentsToLocation.Location.JdeId}.{6011}.{jobTypeToGl}";
                                entryCreator.SetEquipmentHours((decimal)(equipmentEnd.Value.ToTimeSpan().Subtract(equipmentBegin.Value.ToTimeSpan()).TotalHours * 100));
                                entryCreator.SetEquipmentUsedId(userEquipment.Current.Equipment.Sku);
                                entryCreator.OverTimeCode = "R";

                                wasEquipmentHandled = true;
                                isPreOvertimeProcessed = true;
                                if (entryCreator.HoursWorked > 0)
                                {
                                    totalHours -= (double)entryCreator.HoursWorked;
                                    currentLineNumber++;
                                    yield return await entryCreator.CreateTimeEntry(context);
                                }
                            }

                            entryCreator.EdiLineNumber = currentLineNumber * 100;
                            entryCreator.PayrollTransactionNumber = totalEntriesToProcess++;
                            entryCreator.HoursWorked = (decimal)(totalHours * 100);
                            entryCreator.DbaCode = isOvertime ? 100 : 1;
                            entryCreator.OverTimeCode = isOvertime ? "A" : "R";
                            entryCreator.GlAccount = $"{userWorkPeriod.User.DepartmentsToLocation.Location.JdeId}.{(isOvertime ? 6012 : 6011)}.{jobTypeToGl}";
                            if (!wasEquipmentHandled)
                            {
                                entryCreator.SetEquipmentHours((decimal)(equipmentEnd.Value.ToTimeSpan().Subtract(equipmentBegin.Value.ToTimeSpan()).TotalHours * 100));
                                entryCreator.SetEquipmentUsedId(userEquipment.Current.Equipment.Sku);
                            }
                            currentLineNumber++;
                            userEquipment.MoveNext();
                            yield return await entryCreator.CreateTimeEntry(context);
                        }

                        // we processed equipment if any. process time without equipment
                        if (!wasEquipmentProcessed || (equipmentEnd.HasValue && equipmentEnd.Value < jobTypeStepTimeEnd)) //
                        {
                            if (isOvertime && !isPreOvertimeProcessed)
                            {
                                entryCreator.EdiLineNumber = currentLineNumber * 100;
                                entryCreator.PayrollTransactionNumber = totalEntriesToProcess++;
                                entryCreator.HoursWorked = (decimal)((totalHours - 40) * 100);
                                entryCreator.DbaCode = 1;
                                entryCreator.GlAccount = $"{userWorkPeriod.User.DepartmentsToLocation.Location.JdeId}.{6011}.{jobTypeToGl}";
                                entryCreator.OverTimeCode = "R";

                                isPreOvertimeProcessed = true;
                                if (entryCreator.HoursWorked > 0)
                                {
                                    totalHours -= (double)entryCreator.HoursWorked;
                                    currentLineNumber++;
                                    yield return await entryCreator.CreateTimeEntry(context);
                                }
                            }

                            entryCreator.EdiLineNumber = currentLineNumber * 100;
                            entryCreator.PayrollTransactionNumber = totalEntriesToProcess++;
                            entryCreator.HoursWorked = (decimal)(totalHours * 100);
                            entryCreator.DbaCode = isOvertime ? 100 : 1;
                            entryCreator.OverTimeCode = isOvertime ? "A" : "R";
                            entryCreator.GlAccount = $"{userWorkPeriod.User.DepartmentsToLocation.Location.JdeId}.{(isOvertime ? 6012 : 6011)}.{jobTypeToGl}";

                            currentLineNumber++;
                            yield return await entryCreator.CreateTimeEntry(context);
                        }
                    }
                }
                else // no job code for this work period. process using employee defaults
                {
                    if (userEquipment is null)
                    {
                        userEquipment = userWorkPeriod.EquipmentsToUsers.GetEnumerator();
                        userEquipment.MoveNext();
                    }
                    TimeOnly jobTypeStepTimeBegin = (userWorkPeriod.PunchEntries.OrderBy(p => p.CurrentState?.StablePunchEntriesHistory?.DateTime ?? DateTime.MaxValue)
                            .FirstOrDefault(p => p.CurrentState?.StablePunchEntriesHistory?.EffectiveDateTime != null)?.CurrentState?.StablePunchEntriesHistory?.EffectiveDateTime
                        ?? throw new Exception("Could not determine the start of JobTypeStep time")).ToTimeOnly();
                    // get the next job type/step to determine when the begin job type/step expires
                    // if none found, use the last punch for the day, or the end of the day for the begin job type/step
                    TimeOnly jobTypeStepTimeEnd = (userWorkPeriod.PunchEntries.OrderBy(p => p.CurrentState?.StablePunchEntriesHistory?.DateTime ?? DateTime.MinValue)
                            .LastOrDefault(p => p.CurrentState?.StablePunchEntriesHistory?.EffectiveDateTime != null)?.CurrentState?.StablePunchEntriesHistory?.EffectiveDateTime
                        ?? throw new Exception("Could not determine the end of JobTypeStep time")).ToTimeOnly();

                    double totalHours = jobTypeStepTimeEnd.ToTimeSpan().Subtract(jobTypeStepTimeBegin.ToTimeSpan()).TotalHours;
                    employeeHoursTally += totalHours;
                    bool isOvertime = employeeHoursTally > 40;
                    bool isPreOvertimeProcessed = false;
                    string jobTypeToGl = ""; // await TcToJdeEtl.GetGlCodeForJobType(workperiod.JobType);

                    bool wasEquipmentProcessed = false;
                    TimeOnly? equipmentBegin;
                    TimeOnly? equipmentEnd = null;

                    entryCreator.EdiTransactionNumber = transactionNumber;
                    entryCreator.EmployeeId = employeeId;
                    entryCreator.JobType = user.DefaultJobType?.JdeId;
                    entryCreator.JobStep = user.DefaultJobStep?.JdeId;
                    entryCreator.UnionCode = user.UnionCode;

                    // process equipment used
                    while (((equipmentBegin = userEquipment.GetCurrent()?.LinkedOnEffective?.ToTimeOnly()) ?? TimeOnly.MinValue) >= jobTypeStepTimeBegin &&
                        (equipmentEnd = userEquipment.GetCurrent()?.UnLinkedOnEffective?.ToTimeOnly() ?? jobTypeStepTimeEnd) <= jobTypeStepTimeEnd)
                    {
                        bool wasEquipmentHandled = false;
                        wasEquipmentProcessed = true;

                        if (isOvertime && !isPreOvertimeProcessed)
                        {
                            entryCreator.EdiLineNumber = currentLineNumber * 100;
                            entryCreator.PayrollTransactionNumber = totalEntriesToProcess++;
                            entryCreator.HoursWorked = (decimal)((totalHours - 40) * 100);
                            entryCreator.DbaCode = 1;
                            entryCreator.GlAccount = $"{userWorkPeriod.User.DepartmentsToLocation.Location.JdeId}.{6011}.{jobTypeToGl}";
                            entryCreator.SetEquipmentHours((decimal)(equipmentEnd.Value.ToTimeSpan().Subtract(equipmentBegin.Value.ToTimeSpan()).TotalHours * 100));
                            entryCreator.SetEquipmentUsedId(userEquipment.Current.Equipment.Sku);

                            wasEquipmentHandled = true;
                            isPreOvertimeProcessed = true;
                            if (entryCreator.HoursWorked > 0)
                            {
                                totalHours -= (double)entryCreator.HoursWorked;
                                currentLineNumber++;
                                yield return await entryCreator.CreateTimeEntry(context);
                            }
                        }

                        entryCreator.EdiLineNumber = currentLineNumber * 100;
                        entryCreator.PayrollTransactionNumber = totalEntriesToProcess++;
                        entryCreator.HoursWorked = (decimal)(totalHours * 100);
                        entryCreator.DbaCode = isOvertime ? 100 : 1;
                        entryCreator.GlAccount = $"{userWorkPeriod.User.DepartmentsToLocation.Location.JdeId}.{(isOvertime ? 6012 : 6011)}.{jobTypeToGl}";
                        if (!wasEquipmentHandled)
                        {
                            entryCreator.SetEquipmentHours((decimal)(equipmentEnd.Value.ToTimeSpan().Subtract(equipmentBegin.Value.ToTimeSpan()).TotalHours * 100));
                            entryCreator.SetEquipmentUsedId(userEquipment.Current.Equipment.Sku);
                        }
                        currentLineNumber++;
                        userEquipment.MoveNext();
                        yield return await entryCreator.CreateTimeEntry(context);
                    }

                    // we processed equipment if any. process time without equipment
                    if (!wasEquipmentProcessed) // || ((equipmentEnd ?? jobTypeStepTimeEnd) < jobTypeStepTimeEnd)
                    {
                        if (isOvertime && !isPreOvertimeProcessed)
                        {
                            entryCreator.EdiLineNumber = currentLineNumber * 100;
                            entryCreator.PayrollTransactionNumber = totalEntriesToProcess++;
                            entryCreator.HoursWorked = (decimal)((totalHours - 40) * 100);
                            entryCreator.DbaCode = 1;
                            entryCreator.GlAccount = $"{userWorkPeriod.User.DepartmentsToLocation.Location.JdeId}.{6011}.{jobTypeToGl}";

                            isPreOvertimeProcessed = true;
                            if (entryCreator.HoursWorked > 0)
                            {
                                totalHours -= (double)entryCreator.HoursWorked;
                                currentLineNumber++;
                                yield return await entryCreator.CreateTimeEntry(context);
                            }
                        }

                        entryCreator.EdiLineNumber = currentLineNumber * 100;
                        entryCreator.PayrollTransactionNumber = totalEntriesToProcess++;
                        entryCreator.HoursWorked = (decimal)(totalHours * 100);
                        entryCreator.DbaCode = isOvertime ? 100 : 1;
                        entryCreator.GlAccount = $"{userWorkPeriod.User.DepartmentsToLocation.Location.JdeId}.{(isOvertime ? 6012 : 6011)}.{jobTypeToGl}";

                        currentLineNumber++;
                        yield return await entryCreator.CreateTimeEntry(context);
                    }
                }
                #endregion old code with work period job type steps
            }

        }

    }
    private class JdeTimeEntryCreator()
    {
        private bool IsEquipmentHoursSet { get; set; } = false;
        private bool IsEquipmentUsedIdSet { get; set; } = false;
        public string EdiId { get; set; }
        public string EdiTransactionNumber { get; set; }
        public decimal EdiLineNumber { get; set; }
        public int? EdiSubmitDate { get; set; }
        public string EdiBatchNumber { get; set; }
        public int? EmployeeId { get; set; }
        public int? PayrollTransactionNumber { get; set; }
        public decimal? HoursWorked { get; set; }
        public int? DbaCode { get; set; }
        public int? DateWorked { get; set; }
        public int? BatchDate { get; set; }
        public int? DateUpdated { get; set; }
        public string? ProgramId { get; set; }
        public string? GlAccount { get; set; }
        public string? UnionCode { get; set; }
        public string? JobType { get; set; }
        public string? JobStep { get; set; }
        public string? OverTimeCode { get; set; }

        public decimal? EquipmentHours { get; private set; }
        public string? EquipmentUsedId { get; private set; }

        public void SetEquipmentHours(decimal? value)
        {
            this.EquipmentHours = value;
            this.IsEquipmentHoursSet = true;
        }
        public void SetEquipmentUsedId(string? value)
        {
            this.EquipmentUsedId = value;
            this.IsEquipmentUsedIdSet = true;
        }

        public async Task<JdeEmployeeTimeEntryImport> CreateTimeEntry(JdeContext context)
        {
            JdeEmployeeTimeEntryImport timeEntry = new()
            {
                EdiId = this.EdiId, // VLEDUS 
                EdiTransactionNumber = this.EdiTransactionNumber, // VLEDTN
                EdiLineNumber = this.EdiLineNumber, // VLEDLN
                EdiSubmitDate = this.EdiSubmitDate, // VLEDDT
                EdiBatchNumber = this.EdiBatchNumber, // VLEDBT
                EmployeeId = this.EmployeeId, // VLAN8
                PayrollTransactionNumber = this.PayrollTransactionNumber, // VLPRTR
                HoursWorked = this.HoursWorked, // VLPHRW
                DbaCode = this.DbaCode, // VLPDBA - regular time or overtime - Straight time = 1, Overtime = 100
                DateWorked = this.DateWorked, // VLDWK
                BatchDate = this.BatchDate, // VLDICJ
                DateUpdated = this.DateUpdated, // VLUPMJ
                ProgramId = this.ProgramId, // VLPID
                GlAccount = this.GlAccount, // VLANI
                UnionCode = this.UnionCode, // VLUN
                JobStep = this.JobStep, // VLJBST
                JobType = this.JobType, // VLJBCD
                OverTimeCode = this.OverTimeCode, // VLOHF
                //Vluser = app name?,
                //EdiSendReceive = "R" // VLEDER
                //EmployeeName = $"{user.LastName}, {user.FirstName}" // VLALPH
            };

            if (this.IsEquipmentHoursSet)
                timeEntry.EquipmentHours = this.EquipmentHours;
            if (this.IsEquipmentUsedIdSet)
                timeEntry.EquipmentUsedId = this.EquipmentUsedId;

            await context.JdeEmployeeTimeEntryImports.AddAsync(timeEntry);

            this.IsEquipmentHoursSet = false;
            this.IsEquipmentUsedIdSet = false;

            return timeEntry;
        }
    }

    private static async Task<string> GetGlCodeForJobType(JobType? jobType)
    {
        // do something
        return jobType?.JdeId ?? "";
    }

    internal static async Task<int?> GetNextNumber(int nextNumberLength, string connectionString)
    {
        int? currentNext = 0;
        var context = new JdeContext(connectionString);
        JdeNextNumber nextNumberEntity = await context.JdeNextNumbers.FirstAsync(n => n.ProductCode == JdeNextNumber.PayrollTransactionProductCode);
        bool success = false;
        int maxRetries = 5;
        int tries = 0;

        nextNumberLength++;
        nextNumberEntity.NextNumberRange1 += nextNumberLength;

        while (!success && tries < maxRetries)
        {
            try
            {
                context.SaveChanges();
                currentNext = nextNumberEntity.NextNumberRange1;
                success = true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    if (entry.Entity is JdeNextNumber)
                    {
                        var proposedValues = entry.CurrentValues;
                        var databaseValues = entry.GetDatabaseValues();

                        foreach (var property in proposedValues.Properties)
                        {
                            int newVal;
                            var proposedValue = proposedValues[property];
                            // throw if databaseValues is null
                            string? databaseValue = databaseValues![property]?.ToString();

                            if (property.Name == nameof(JdeNextNumber.NextNumberRange1) && int.TryParse(databaseValue, out newVal))
                            {
                                currentNext = newVal + nextNumberLength;
                                proposedValues[property] = currentNext;
                            }
                        }

                        // Refresh original values to bypass next concurrency check
                        entry.OriginalValues.SetValues(databaseValues!);
                    }
                }
            }
            tries++;
        }

        if (!success)
            return null;

        //while (success < 1)
        //{
        //    success = await this.JdeContext.JdeNextNumbers
        //        .Where(n => n.ProductCode == JdeNextNumber.PayrollTransactionProductCode && n.NextNumberRange1 == currentNext)
        //        .ExecuteUpdateAsync(n => n.SetProperty(p => p.NextNumberRange1, p => p.NextNumberRange1 + nextNumberLength));
        //    nextNumberEntity = await this.JdeContext.JdeNextNumbers.FirstAsync(n => n.ProductCode == JdeNextNumber.PayrollTransactionProductCode);
        //    currentNext = nextNumberEntity.NextNumberRange1;
        //}

        return currentNext - nextNumberLength + 1;
    }

    private static async Task<int> GetNumberOfTimeEntries(TimeClockContext tcContext, DateOnly minDate, DateOnly maxDate, ILogger logger, CancellationToken cancellationToken)
    {
        try
        {
            string sql = @"
SELECT @total = COUNT(*) FROM (
	SELECT w.UserId, 
			CASE WHEN SUM(w.HoursWorked) > 40 THEN 40 ELSE SUM(w.HoursWorked) END HoursWorked, 
			0 IsOvertime
		FROM timeclock.WorkPeriods w
		LEFT JOIN timeclock.WorkPeriodJobTypeSteps j ON j.WorkPeriodId = w.Id
		--WHERE w.WorkDate >= @minDate AND w.WorkDate <= @maxDate
		GROUP BY w.UserId, j.Id
	UNION ALL
	(
		SELECT w2.UserId, SUM(w2.HoursWorked) - 40 HoursWorked, 1 IsOvertime
		FROM timeclock.WorkPeriods w2
		LEFT JOIN timeclock.WorkPeriodJobTypeSteps j2 ON j2.WorkPeriodId = w2.Id
		--WHERE w2.WorkDate >= @minDate AND w2.WorkDate <= @maxDate
		GROUP BY w2.UserId, j2.Id
		HAVING SUM(w2.HoursWorked) > 40
	)
	--ORDER BY UserId, hoursWorked DESC
) z
";
            SqlParameter[] sqlParameters = [
                //new("@minDate", minDate),
                //new("@maxDate", maxDate),
                new("@total", System.Data.SqlDbType.Int)
                {
                    Direction = System.Data.ParameterDirection.Output
                }
            ];
            await tcContext.Database.ExecuteSqlRawAsync(sql, sqlParameters, cancellationToken);

            return (int)sqlParameters.First(p => p.ParameterName == "@total").Value;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "GetNumberOfTimeEntries failed. {0}");
            throw;
        }
    }

    private static async Task OutputEntryToFile(JdeEmployeeTimeEntryImport timeEntry, JdeContext context, string fileName, ILogger logger, CancellationToken cancellationToken)
    {
        try
        {
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(docPath, fileName);
            bool needsHeaders = !File.Exists(filePath);
            StringBuilder? headerLine = null;
            StringBuilder dataLine = new();

            foreach (PropertyInfo prop in timeEntry.GetType().GetProperties())
            {
                if (needsHeaders)
                {
                    if (headerLine is null) headerLine = new();
                    headerLine.AppendTabbed(context.Model.FindEntityType(typeof(JdeEmployeeTimeEntryImport))!.GetProperty(prop.Name).GetColumnName());
                }

                dataLine.AppendTabbed(context.Model.FindEntityType(typeof(JdeEmployeeTimeEntryImport))!.GetProperty(prop.Name).FieldInfo.GetValue(timeEntry)?.ToString() ?? "NULL");
            }

            using (StreamWriter outputFile = new(filePath))
            {
                if (needsHeaders)
                {
                    headerLine!.AppendLine(dataLine.ToString());
                    await outputFile.WriteLineAsync(headerLine, cancellationToken);
                }
                else
                {
                    await outputFile.WriteLineAsync(dataLine, cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex, "OutputEntryToFile failed. {0}");
            throw;
        }
    }




    #region IDisposable
    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                try { this.TcContext.Dispose(); } catch (Exception) { }
                try { this.JdeContext.Dispose(); } catch (Exception) { }
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
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
}


