using TimeClock.Maui.ViewModels.Shared;

namespace TimeClock.Maui.ViewModels;

public class HistoryViewModel : ViewModelBase
{

    protected override Task Refresh()
    {
        return Task.CompletedTask;
    }
}
