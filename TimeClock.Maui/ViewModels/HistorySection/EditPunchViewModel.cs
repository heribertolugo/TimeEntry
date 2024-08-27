using CommunityToolkit.Mvvm.ComponentModel;
using TimeClock.Maui.Models;

namespace TimeClock.Maui.ViewModels.HistorySection
{
    public class EditPunchViewModel : ObservableObject
    {
        private Guid _userId;
        private Guid _punchId;
        private TimeSpan _newPunchTime;
        private Time _punchTime;
        private string _reason;
        public EditPunchViewModel() {
            this._userId = Guid.Empty;
            this._punchId = Guid.Empty;
            this._newPunchTime = TimeSpan.Zero;
            this._punchTime = TimeSpan.Zero;
            this._reason = string.Empty;
        }
        public Guid UserId 
        { 
            get => this._userId; 
            set => base.SetProperty(ref this._userId, value); 
        }
        public Guid PunchId 
        { 
            get => this._punchId; 
            set => base.SetProperty(ref this._punchId, value);
        }
        public Time PunchTime
        {
            get => this._punchTime;
            set => base.SetProperty(ref this._punchTime, value);
        }
        public TimeSpan NewPunchTime
        {
            get => this._newPunchTime;
            set => base.SetProperty(ref this._newPunchTime, value);
        }
        public string Reason 
        { 
            get => this._reason; 
            set => base.SetProperty(ref this._reason, value); 
        }
        public bool IsNotNew
        {
            get => this.PunchTime != default;
        }
    }
}
