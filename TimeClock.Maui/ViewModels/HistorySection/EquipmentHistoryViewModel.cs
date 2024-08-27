using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using TimeClock.Core;
using TimeClock.Core.Models;
using TimeClock.Core.Models.ApiDtos;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Maui.Helpers;
using TimeClock.Maui.Services;
using TimeClock.Maui.ViewModels.Shared;

namespace TimeClock.Maui.ViewModels.HistorySection
{
    internal sealed class EquipmentHistoryViewModel : ViewModelBase
    {
        private ObservableCollection<EquipmentsToUserDto> _equipments;
        private ITimeClockApiAccessService ApiAccessService { get; init; }
        private ILogger<EquipmentHistoryViewModel> Logger { get; init; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public EquipmentHistoryViewModel(ITimeClockApiAccessService apiAccessService, ILogger<EquipmentHistoryViewModel> logger)
        {
            this.SetDefaults();
            this.ApiAccessService = apiAccessService;
            this.Logger = logger;
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        #region Public Properties
        public ObservableCollection<EquipmentsToUserDto> Equipments
        {
            get => this._equipments;
            set => base.SetProperty(ref this._equipments, value);
        }
        #endregion Public Properties


        #region Public Commands
        public IAsyncRelayCommand DelayLoadMoreCommand { get; set; }
        #endregion Public Commands


        #region Private Command Actions
        protected override Task Refresh()
        {
            return this.GetSetEquipmentsAsync();
        }
        #endregion Private Command Actions

        
        #region Private Methods/Helpers
        private void SetDefaults()
        {
            base._filterDate = DateTime.Now;
            this.Equipments = new ObservableCollection<EquipmentsToUserDto>(Enumerable.Empty<EquipmentsToUserDto>());
        }
        private async Task GetSetEquipmentsAsync()
        {
            if (base.IsBusy)
            {
                await UiHelpers.ShowToast(CommonValues.AppBusy);
                return;
            }

            base.IsBusy = true;

            try
            {
                ResultValues<IEnumerable<EquipmentDto>?> getEquipment = await this.GetEquipmentsAsync(this.UserId, this.FilterDate, this.FilterDate);
                if (getEquipment.IsSuccessfulStatusCode && getEquipment.Value is not null)
                    await this.SetEquipmentsAsync(getEquipment.Value);
                else
                {
                    await UiHelpers.ShowOkAlert($"Unable to get equipment history. {getEquipment.StatusCode}");
                    this.Logger.LogError("Unable to get equipment history. status code: {ApiResultCode}", getEquipment.StatusCode);
                }
            }
            catch(Exception ex)
            {
                await UiHelpers.ShowOkAlert($"Fatal error when trying to get equipment history.\nContact IT.");
                this.Logger.LogError(ex, "Fatal error when trying to get equipment history. exception: {exception}", ex.Message);
            }
            finally
            {
                base.IsBusy = false;
                base.IsRefreshing = false;
            }
        }
        private Task<ResultValues<IEnumerable<EquipmentDto>?>> GetEquipmentsAsync(Guid userId, DateTime from, DateTime to)
        {
            GetEquipmentsDto data = new()
            {
                UserId = userId,
                LinkedDateRange = new DateRange(from.StartOfDay(), to.EndOfDay()),
                IncludeEquipmentToUserUser = true,
                HistoryForUserAndWorkPeriod = new GetEquipmentsDto.UserAndWorkPeriod(userId, from.Date)
            };
            data.Sorting.Add(new SortOption<GetEquipmentsDto.SortField>(GetEquipmentsDto.SortField.LastUsed, Core.Models.SortOrderDto.Descending));

            return this.ApiAccessService.GetEquipment(Settings.DeviceGuid, data);
        }
        private ValueTask SetEquipmentsAsync(IEnumerable<EquipmentDto> equipment)
        {
            this.Equipments.Clear();
            EquipmentsToUserDto[] eqToUser = equipment.SelectMany(q =>
            {
                foreach (var item in q.EquipmentsToUsers)
                    item.Equipment = q;
                return q.EquipmentsToUsers;
            }).ToArray();

            foreach (var dto in eqToUser)
            {
                this.Equipments.Add(dto);
            }

            return ValueTask.CompletedTask;
        }
        #endregion Private Methods/Helpers
    }
}
