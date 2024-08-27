using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TimeClock.Core;
using TimeClock.Core.Models;
using TimeClock.Core.Models.ApiDtos;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Maui.Helpers;
using TimeClock.Maui.Services;
using TimeClock.Maui.ViewModels.Shared;

namespace TimeClock.Maui.ViewModels.AdminSection
{
    internal sealed class PunchSummaryViewModel : ViewModelBase
    {
        #region Private Members
        private IList<ActivitySummary> _punchSummaries;
        private DateTime _lastFetchedAt;
        private IEnumerable<UserDto> _users;
        private IDictionary<UserDto, IEnumerable<PunchEntryDto>> _usersPunches;
        private IDictionary<UserDto, IDictionary<DateTime, EquipmentDto>> _userEquipments;
        private ITimeClockApiAccessService ApiService { get; set; }
        #endregion Private Members

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public PunchSummaryViewModel(ITimeClockApiAccessService apiService)
        {
            this.ApiService = apiService;
            this.SetDefaults();
            this.SetCommands();
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        #region Public Properties
        public IList<ActivitySummary> ActivitySummaries
        {
            get => this._punchSummaries;
            private set => base.SetProperty(ref this._punchSummaries, value);
        }
        public DateTime LastFetchedAt
        {
            get => this._lastFetchedAt;
            private set => base.SetProperty(ref this._lastFetchedAt, value);
        }
            
        #endregion Public Properties


        #region Public Commands
        public ICommand LoadPreviousDateCommand { get; private set; }
        public ICommand LoadNextDateCommand { get; private set; }
        public IAsyncRelayCommand DelayLoadMoreCommand { get; private set; }
        #endregion Public Commands


        #region Private Command Actions
        protected override Task Refresh()
        {
            return this.GetAndLoadUsersPunched(this.FilterDate);
        }
        private void LoadPreviousDate()
        {
            this.FilterDate = this.FilterDate.AddDays(-1);            
        }
        private void LoadNextDate()
        {
            this.FilterDate = this.FilterDate.AddDays(1);
        }
        #endregion Private Command Actions


        #region Private Methods/Helpers
        private void SetDefaults()
        {
            this._punchSummaries = new ObservableCollection<ActivitySummary>();
            this._lastFetchedAt = DateTime.MinValue;
            this._users = Enumerable.Empty<UserDto>();
            this._usersPunches = new Dictionary<UserDto, IEnumerable<PunchEntryDto>>();
            this._userEquipments = new Dictionary<UserDto, IDictionary<DateTime, EquipmentDto>>();
        }

        private void SetCommands()
        {
            this.LoadPreviousDateCommand = new Command(this.LoadPreviousDate);
            this.LoadNextDateCommand = new Command(this.LoadNextDate);
            this.RefreshCommand = new AsyncRelayCommand(this.Refresh);
        }

        private async Task GetAndLoadUsersPunched(DateTime date)
        {
            if (this.UserId == default)
                return;

            if (base.IsBusy)
            {
                await UiHelpers.ShowToast(CommonValues.AppBusy);
                return;
            }

            base.IsBusy = true;

            //GetPunchEntriesDto data = new()
            //{
            //    //DepartmentId = Settings.DepartmentId,
            //    //LocationId = Settings.LocationId,
            //    DeviceId = Settings.DeviceGuid,
            //    DateRange = new DateRange(date,date),
            //    RequestedById = this.UserId,
            //    IncludeUser = true,
            //    IncludeEquipment = true
            //};
            //data.Sorting.Add(new SortOption<GetPunchEntriesDto.SortField>(GetPunchEntriesDto.SortField.DateTime, SortOrderDto.Descending));

            //var punches = await this._apiService.GetPunches(Settings.DeviceGuid, null, data);

            GetWorkPeriodsDto data = new()
            {
                DateRange = new DateRange(date.StartOfDay(),date.EndOfDay()),
                DeviceId = Settings.DeviceGuid,
                IncludeUser = true,
                RequestedById = this.UserId,
                IncludeEquipment = true,
                IncludePunchEntries = true,
                PunchActions = new(PunchActionDtoEx.PositiveStates())
            };

            var punches = await this.ApiService.GetWorkPeriods(Settings.DeviceGuid, data, (await Settings.GetCryptoApiKey()) ?? string.Empty);

            if (!punches.IsSuccessfulStatusCode || punches.Value is null)
            {
                base.IsBusy = false;
                base.IsRefreshing = false;
#warning log and show user alert
                return;
            }

            this.ActivitySummaries.Clear();
            var summary = new List<ActivitySummary>(punches.Value.GroupBy(p => p.UserId).Select(g => 
                new ActivitySummary(
                    g.Select(p => p.User).FirstOrDefault() ?? new(), 
                    g.SelectMany(w => w.PunchEntries), 
                    g.SelectMany(g => g.EquipmentsToUsers))
                ));
            this.LastFetchedAt = DateTime.Now;

            this.ActivitySummaries = summary;

            base.IsBusy = false;
            base.IsRefreshing = false;
        }
        private void SetUserPunched(DateTime date)
        {

        }

        private void GetUsersPunchedForDate(DateTime date)
        {

        }
        #endregion Private Methods/Helpers
    }

    internal record ActivitySummaryPunch(string Duration, string Total);

    internal record ActivitySummaryEquipment(string Sku, string Name, string Duration);

    internal class ActivitySummary
    {
        private UserDto _user;
        private IEnumerable<PunchEntryDto> _punches;
        private IEnumerable<EquipmentsToUserDto> _equipmentToUser;

        public ActivitySummary(UserDto user, IEnumerable<PunchEntryDto> punches, IEnumerable<EquipmentsToUserDto> equipmentToUser) 
        { 
            this._user = user;
            this._punches = punches;
            this._equipmentToUser = equipmentToUser;
            this.UserFullName = $"{this._user.FirstName} {this._user.LastName}";
            this.CreatePunchesSummary();
            this.CreateEquipmentSummary();
        }

        public string UserFullName
        {
            get; private set;
        }

        public bool HasEquipment
        {
            get; private set;
        }

        public bool IsPunchedIn
        {
            get; private set;
        }

        public List<ActivitySummaryPunch> PunchSummaries { get; set; } = [];

        public List<ActivitySummaryEquipment> EquipmentSummaries { get; set; } = [];

        private void CreateEquipmentSummary()
        {
            var equipments = this._equipmentToUser.OrderBy(q => q.LinkedOn);

            foreach (var equipment in equipments)
                this.EquipmentSummaries.Add(new ActivitySummaryEquipment($"{equipment.Equipment?.Sku},", $"{equipment.Equipment?.Name}", $"{equipment.LinkedOnEffective?.ToShortTimeString()}-{equipment.UnLinkedOnEffective?.ToShortTimeString()}"));

            this.HasEquipment = this.EquipmentSummaries.Count > 0;
        }

        private void CreatePunchesSummary()
        {
            var punchPairs = this._punches.OrderBy(p => p.EffectiveDateTime).Chunk(2);
            double total = 0;

            foreach(var pair in punchPairs)
            {
                if (pair.Length < 2)
                {
                    this.PunchSummaries.Add(new ActivitySummaryPunch($"{pair[0]?.EffectiveDateTime.ToShortTimeString()}-", string.Empty));
                    this.IsPunchedIn = true;
                    break;
                }
                var chunkTotal = pair[1].EffectiveDateTime.Subtract(pair[0].EffectiveDateTime).TotalHours;
                total += chunkTotal;
                this.PunchSummaries.Add(new ActivitySummaryPunch($"{pair[0]?.EffectiveDateTime.ToShortTimeString()}-{pair[1]?.EffectiveDateTime.ToShortTimeString()}", $"({chunkTotal:N2} Hours)"));
            }

            this.PunchSummaries.Add(new ActivitySummaryPunch(string.Empty, $"Total: {total:N2}"));
        }
    }
}
