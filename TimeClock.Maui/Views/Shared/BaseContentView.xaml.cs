using TimeClock.Maui.Helpers;
using TimeClock.Maui.ViewModels.Shared;

namespace TimeClock.Maui.Views.Shared;

public partial class BaseContentView : ContentView
{
    public static readonly BindableProperty IsNotMobileDeviceProperty = BindableProperty.Create(nameof(IsNotMobileDevice), typeof(bool), typeof(BaseContentView));
    public static readonly BindableProperty UserIdProperty = BindableProperty.Create(nameof(UserId), typeof(Guid), typeof(BaseContentView),
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (BaseContentView)bindable;
            if (control?.ViewModel is not null && newValue is not null)
                control.ViewModel.UserId = (Guid)newValue;
        });
    public static readonly BindableProperty FilterDateProperty = BindableProperty.Create(nameof(FilterDate), typeof(DateTime), typeof(BaseContentView),
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (BaseContentView)bindable;
            if (control?.ViewModel is not null && newValue is not null)
                control.ViewModel.FilterDate = (DateTime)newValue;
        });
    public static readonly BindableProperty UserJobTypeIdProperty = BindableProperty.Create(nameof(UserJobTypeId), typeof(Guid?), typeof(BaseContentView),
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (BaseContentView)bindable;
            if (control?.ViewModel is not null)
                control.ViewModel.UserJobTypeId = (Guid?)newValue;
        });
    public static readonly BindableProperty UserJobStepIdProperty = BindableProperty.Create(nameof(UserJobStepId), typeof(Guid?), typeof(BaseContentView),
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (BaseContentView)bindable;
            if (control?.ViewModel is not null)
                control.ViewModel.UserJobStepId = (Guid?)newValue;
        });
    public static readonly BindableProperty PunchTypeProperty = BindableProperty.Create(nameof(PunchType), typeof(int), typeof(BaseContentView),
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (BaseContentView)bindable;
            if (control?.ViewModel is not null)
                control.ViewModel.PunchType = (int)newValue;
        });
    private bool _isNotMobileDevice;

    public BaseContentView()
    {
        this.InitializeComponent();
        ViewModel = default;
        _isNotMobileDevice = !DeviceInfo.Current.IsMobile();
    }


    public virtual DateTime FilterDate
    {
        get => (DateTime)GetValue(FilterDateProperty);
        set
        {
            SetValue(FilterDateProperty, value);
            if (ViewModel is not null)
                ViewModel.FilterDate = value;
        }
    }

    public virtual Guid UserId
    {
        get => (Guid)GetValue(UserIdProperty);
        set
        {
            SetValue(UserIdProperty, value);
            if (ViewModel is not null)
                ViewModel.UserId = value;
        }
    }

    public bool IsNotMobileDevice
    {
        get => (bool)GetValue(IsNotMobileDeviceProperty);
        set => SetValue(IsNotMobileDeviceProperty, _isNotMobileDevice);
    }
    public int PunchType
    {
        get => (int)GetValue(PunchTypeProperty);
        set
        {
            SetValue(PunchTypeProperty, value);
            if (ViewModel is not null)
                ViewModel.PunchType = value;
        }
    }

    public virtual Guid? UserJobTypeId
    {
        get => (Guid?)GetValue(UserJobTypeIdProperty);
        set
        {
            SetValue(UserJobTypeIdProperty, value);
            if (ViewModel is not null)
                ViewModel.UserJobTypeId = value;
        }
    }

    public virtual Guid? UserJobStepId
    {
        get => (Guid?)GetValue(UserJobStepIdProperty);
        set
        {
            SetValue(UserJobStepIdProperty, value);
            if (ViewModel is not null)
                ViewModel.UserJobStepId = value;
        }
    }

    protected virtual IViewModel? ViewModel { get; set; }
    protected void SetViewModelBinding<T>() where T : class, IViewModel
    {
        ViewModel = ServiceHelper.GetService<T>() ?? throw new ApplicationException(string.Format(CommonValues.FatalNotFound, typeof(T).Name));
        BindingContext = ViewModel;
    }
}