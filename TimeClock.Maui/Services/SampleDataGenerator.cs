using TimeClock.Core;
using TimeClock.Core.Models;
using TimeClock.Core.Models.ApiDtos;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Maui.Helpers;

namespace TimeClock.Maui.Services;

internal class SampleDataGenerator
{
    private static string AdminBarcode = "789123456";
    private Guid DeviceId { get; set; }
    private Guid LocationId { get; set; }
    private Guid DepartmentId { get; set; }
    private ITimeClockApiAccessService ApiService { get; set; }
    private DateTime From { get; set; } = new DateTime(2024,8,1);
    private DateTime To { get; set; } = new DateTime(2024,8,25);
    private Random Random { get; set; } = new Random();
    //private StringBuilder StringBuilder { get; set; } = new();
    private UserDto? Admin { get; set; }

    public SampleDataGenerator(ITimeClockApiAccessService apiService)
    {
        this.DeviceId = Settings.DeviceGuid;
        this.LocationId = Settings.LocationId;
        this.DepartmentId = Settings.DepartmentId;
        this.ApiService = apiService;
    }

    public async Task Generate()
    {
        try
        {
            await this.WriteToFile(" ", reset: true);
            await this.Init();

            await this.WriteToFile($"Init complete. Creating entries from {this.From} to {this.To}");

            foreach (DateTime dateTime in this.From.StartOfDay().UntilDate(this.To))
            {
                foreach (UserDto user in this.UsersAtLocation)
                {
                    await this.WriteToFile($"{Environment.NewLine}{Environment.NewLine}User: {user.FullNameOr}; JdeID {user.JdeId}; {dateTime:D}");
                    await this.CreatePunchesForDay(user, dateTime);
                }
            } 
        }catch(Exception ex)
        {
            await this.WriteToFile("Exception: ");
            await this.WriteToFile(ex.GetType()?.FullName ?? string.Empty);
            await this.WriteToFile("Source : " + ex.Source);
            await this.WriteToFile("Message : " + ex.Message);
            await this.WriteToFile("StackTrace : " + ex.StackTrace);
            await this.WriteToFile("InnerException : " + ex.InnerException?.Message);
        }
    }

    private async Task Init()
    {
        ValidateClaimDto data = new()
        {
            Claims = UiPageMeta.Admin.Claims,
            DeviceId = Settings.DeviceGuid,
            Password = "",
            UserName = "",
            PunchTypeDto = PunchTypeDto.Domain
        };

        string crypto = await Settings.GetCryptoApiKey() ?? string.Empty;

        var result = (await this.ApiService.RequestAccess(Settings.DeviceGuid, crypto, data));
        if (result is null || !result.IsSuccessfulStatusCode)
            Console.WriteLine();
        await Task.WhenAll(
            this.LoadUsers(),
            this.LoadEquipment()
            );

        await this.EnsureAdminBarcode();
    }

    private async Task LoadUsers()
    {
        if (this.UsersAtLocation.Any()) return;

        GetUsersDto data = new()
        {
            DepartmentId = this.DepartmentId,
            LocationId = this.LocationId,
            DeviceId = this.DeviceId,
            IsActive = true,
            IncludeEquipmentToUser = false,
            IncludeClaims = true,
            IncludeJobType = true,
            IncludeJobStep = true,
            UserActiveState = true,
            IncludeBarCode = true,
            IncludeJobTypeSteps = false,
            Paging = new PagingDto(1, 100)
        };

        ResultValues<IEnumerable<UserDto>?> result = await this.ApiService.GetUsers(data.DeviceId, data);

        this.UsersAtLocation = new(result.Value);
    }

    private async Task LoadEquipment()
    {
        if (this.EquipmentAtLocation.Any()) return;

        GetEquipmentsDto data = new()
        {
            DepartmentId = Settings.DepartmentId,
            LocationId = Settings.LocationId,
            IsActive = true,
            IncludeCurrentEquipmentToUser = true,
            IncludeEquipmentToUserUser = true,
            IncludeEquipmentJobTypeSteps = true
        };

        ResultValues<IEnumerable<EquipmentDto>?> result = await this.ApiService.GetEquipment(Settings.DeviceGuid, data);

        this.EquipmentAtLocation = new(result.Value);
    }

    private async Task CreatePunchesForDay(UserDto user, DateTime date)
    {
        if (this.Random.Next(0, 10) == 0)
            return;

        int[] punchCounts = [4, 2, 4, 4];
        int totalPunches = punchCounts[this.Random.Next(0, punchCounts.Length)];
        int punchesCreatedCount = 0;
        DateTime punchDateTime = date.AddHours(this.Random.Next(7, 11)).AddMinutes(this.Random.Next(0, 31));
        EquipmentsToUserDto? selectedEquipmentToUser = null;

        while(punchesCreatedCount < totalPunches)
        {
            bool punchingIn = punchesCreatedCount % 2 == 0;
            // only a punch-in can select equipment. select equipment randomly if punching in.
            bool hasEquipment = punchingIn && this.Random.Next(0, 10) <= 1 && false;
            bool isEquipmentWithPunch = hasEquipment && this.Random.Next(0,2) == 1;

            if (hasEquipment)
            {
                if (!isEquipmentWithPunch)
                {
                    await this.WriteToFile($"Punch In: {punchDateTime:t}");
                    await this.CreatePunchEntry(user, punchDateTime);
                    punchDateTime = punchDateTime.AddMinutes(this.Random.Next(15, 120));
                }
                else
                {
                    if (selectedEquipmentToUser is not null)
                    {
                        await this.UnlinkEquipmentToUser(user, punchDateTime, selectedEquipmentToUser, true);
                        await this.WriteToFile($"Un-selected Equipment: {selectedEquipmentToUser?.Equipment?.Name} {selectedEquipmentToUser?.EquipmentId} {punchDateTime:t}");
                        selectedEquipmentToUser = null;
                    }
                }

                selectedEquipmentToUser = await this.AddEquipmentToUser(user, punchDateTime);
                await this.WriteToFile($"Selected Equipment: {selectedEquipmentToUser?.Equipment?.Name} {selectedEquipmentToUser?.EquipmentId} {punchDateTime:t}");
            }
            else
            {
                if (selectedEquipmentToUser is not null)
                {
                    await this.UnlinkEquipmentToUser(user, punchDateTime, selectedEquipmentToUser, true);
                    await this.WriteToFile($"Un-selected Equipment: {selectedEquipmentToUser?.Equipment?.Name} {selectedEquipmentToUser?.EquipmentId} {punchDateTime:t}");
                    selectedEquipmentToUser = null;
                }
                else
                {
                    await this.WriteToFile($"Punch {(punchingIn ? "In" : "Out")}: {punchDateTime:t}");
                    await this.CreatePunchEntry(user, punchDateTime);
                }
            }
            // if we were punched in, out punch out should be a few hours after
            punchDateTime = punchingIn ? punchDateTime.AddHours(this.Random.Next(3, 5)) : punchDateTime.AddMinutes(this.Random.Next(30, 60));
            punchesCreatedCount++;
        }
    }
    private async Task<EquipmentsToUserDto?> AddEquipmentToUser(UserDto user, DateTime dateTime)
    {
        SlimPunchEntry punchEntry = new()
        {
            DateTime = dateTime,
            PunchAction = PunchActionDto.AdminEquipmentSelect,
            JobTypeId = user.DefaultJobTypeId,
            JobStepId = user.DefaultJobStepId,
            Latitude = null,
            Longitude = null,
            PunchType = PunchTypeDto.None,
            UserId = user.Id
        };

        EquipmentDto? equipment = this.ConsumeAnEquipment();

        if (equipment is null) return null;

        var result = await this.ApiService.LinkUserToEquipment(Settings.DeviceGuid, await Settings.GetCryptoApiKey() ?? string.Empty,
                new(Settings.DeviceGuid, user.Id, equipment.Id, user.Id, user.DefaultJobTypeId, user.DefaultJobStepId, punchEntry));
        return result.IsSuccessfulStatusCode ? result.Value : null;
    }
    private async Task<EquipmentsToUserDto?> UnlinkEquipmentToUser(UserDto user, DateTime dateTime, EquipmentsToUserDto equipmentsToUser, bool punchingOut)
    {
        SlimPunchEntry punchEntry = new()
        {
            DateTime = dateTime,
            PunchAction = PunchActionDto.AdminEquipmentSelect,
            JobTypeId = user.DefaultJobTypeId,
            JobStepId = user.DefaultJobStepId,
            Latitude = null,
            Longitude = null,
            PunchType = PunchTypeDto.None,
            UserId = user.Id
        };

        EquipmentDto? equipment = this.ConsumeAnEquipment();

        if (equipment is null) return null;

        var result = await this.ApiService.UnlinkUserToEquipment(Settings.DeviceGuid, await Settings.GetCryptoApiKey() ?? string.Empty,
                                    new(Settings.DeviceGuid, equipmentsToUser.Id, user.Id, equipmentsToUser.EquipmentId,
                                    user.Id, punchingOut, punchEntry, user.DefaultJobTypeId, user.DefaultJobStepId));
        return result.IsSuccessfulStatusCode ? result.Value : null;
    }

    private EquipmentDto? ConsumeAnEquipment()
    {
        if (!this.EquipmentAtLocation.Any()) return null;

        int index = this.Random.Next(0, this.EquipmentAtLocation.Count);

        EquipmentDto q = this.EquipmentAtLocation[index];
        this.EquipmentAtLocation.RemoveAt(index);
        return q;
    }

    private async Task CreatePunchEntry(UserDto user, DateTime dateTime)
    {
        CreatePunchEntryDto punchEntry = new()
        {
            ActionById = this.Admin.Id,
            PunchAction = PunchActionDto.AdminPunch,
            DateTime = dateTime,
            DeviceId = Settings.DeviceGuid,
            Id = Guid.NewGuid(),
            Password = null,
            PunchType = PunchTypeDto.Barcode,
            IncludeUser = true,
            LocationDivisionCode = Settings.LocationDivision,
            UnionCode = user.UnionCode,
            UserId = user.Id,
            JobTypeId = user.DefaultJobTypeId,
            JobStepId = user.DefaultJobStepId,
            IsJobTypeStepSet = true,

        };

        if (user.UserName is not null)
            punchEntry.UserName = user.UserName;

        var result = await this.ApiService.CreatePunchEntry(punchEntry.DeviceId, (await Settings.GetCryptoApiKey()) ?? string.Empty, punchEntry);
    }

    private async Task EnsureAdminBarcode()
    {
        if (this.Admin is not null) return;

        var admins = this.UsersAtLocation.Where(u => u.IsActive && u.UserClaims.Any(c => c.AuthorizationClaim?.Type == Enum.GetName(AuthorizationClaimType.CanEditOthersPunches)) && u.Barcodes.Any());
        
        if (!admins.Any())
            admins = this.UsersAtLocation.Where(u => u.IsActive && u.UserClaims.Any(c => c.AuthorizationClaim?.Type == Enum.GetName(AuthorizationClaimType.CanEditOthersPunches)));

        int adminIndex = this.Random.Next(0, admins.Count());

        this.Admin = admins.ElementAt(adminIndex);

        if (!this.Admin.Barcodes.Any())
        {
            UpdateUserDto dto = new()
            {
                DeviceId = Settings.DeviceGuid,
                UserId = this.Admin.Id,
                ActionById = this.Admin.Id,
                Barcode = AdminBarcode,
                Username = this.Admin.UserName
            };

            var result = await this.ApiService.UpdateUser(Settings.DeviceGuid, dto);
        }
        else
        {
            string? temp = this.Admin.Barcodes.FirstOrDefault()?.Value;

            if (temp is not null)
                AdminBarcode = temp;
            else
            {
                this.Admin = null;
                await this.EnsureAdminBarcode();
            }
        }
    }

    private async Task WriteToFile(string? text = null, bool reset = false)
    {
        string targetFileName = $"{this.From:MMddyyy}_{this.To:MMddyyyy}.sample";
        // Write the file content to the app data directory  
        string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, targetFileName);
        if (!reset)
            await File.AppendAllTextAsync(targetFile, $"{text}{Environment.NewLine}");
        else
        {
            await File.WriteAllTextAsync(targetFile, $"{text}{Environment.NewLine}");
            //using FileStream outputStream = System.IO.File.OpenWrite(targetFile);
            //using StreamWriter streamWriter = new StreamWriter(outputStream);
            //await streamWriter.WriteAsync($"{text}{Environment.NewLine}");
        }
        System.Diagnostics.Trace.WriteLine(text);
    }

    private List<UserDto> UsersAtLocation { get; set; } = [];
    private List<EquipmentDto> EquipmentAtLocation { get; set; } = [];
}
