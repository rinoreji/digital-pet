using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DigitalPetApp.Services;

namespace DigitalPetApp.ViewModels;

public class SettingsViewModel : INotifyPropertyChanged
{
    private readonly ISettingsService settingsService;
    private readonly IFeatureHost featureHost;
    private readonly ActivityMonitor activityMonitor;

    public SettingsViewModel(ISettingsService settingsService, IFeatureHost featureHost, ActivityMonitor activityMonitor)
    {
        this.settingsService = settingsService;
        this.featureHost = featureHost;
        this.activityMonitor = activityMonitor;
        // copy current values
        idleTimeoutSeconds = settingsService.Current.IdleTimeoutSeconds;
        reminderIntervalMinutes = settingsService.Current.ReminderIntervalMinutes;
        enableReminder = settingsService.Current.EnableReminder;
        enableHourlyChime = settingsService.Current.EnableHourlyChime;
        enableIdleAnimation = settingsService.Current.EnableIdleAnimation;
    enableRandomTrick = settingsService.Current.EnableRandomTrick;
    SaveCommand = new RelayCommand(_ => Save());
    CancelCommand = new RelayCommand(_ => CloseWindow());
    }

    private int idleTimeoutSeconds;
    public int IdleTimeoutSeconds { get => idleTimeoutSeconds; set { if (value!=idleTimeoutSeconds){ idleTimeoutSeconds=value; OnPropertyChanged(); } } }

    private int reminderIntervalMinutes;
    public int ReminderIntervalMinutes { get => reminderIntervalMinutes; set { if (value!=reminderIntervalMinutes){ reminderIntervalMinutes=value; OnPropertyChanged(); } } }

    private bool enableReminder;
    public bool EnableReminder { get => enableReminder; set { if (value!=enableReminder){ enableReminder=value; OnPropertyChanged(); } } }
    private bool enableHourlyChime;
    public bool EnableHourlyChime { get => enableHourlyChime; set { if (value!=enableHourlyChime){ enableHourlyChime=value; OnPropertyChanged(); } } }
    private bool enableIdleAnimation;
    public bool EnableIdleAnimation { get => enableIdleAnimation; set { if (value!=enableIdleAnimation){ enableIdleAnimation=value; OnPropertyChanged(); } } }
    private bool enableRandomTrick;
    public bool EnableRandomTrick { get => enableRandomTrick; set { if (value!=enableRandomTrick){ enableRandomTrick=value; OnPropertyChanged(); } } }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    private void Save()
    {
        settingsService.Current.IdleTimeoutSeconds = IdleTimeoutSeconds;
        settingsService.Current.ReminderIntervalMinutes = ReminderIntervalMinutes;
        settingsService.Current.EnableReminder = EnableReminder;
        settingsService.Current.EnableHourlyChime = EnableHourlyChime;
        settingsService.Current.EnableIdleAnimation = EnableIdleAnimation;
    settingsService.Current.EnableRandomTrick = EnableRandomTrick;
        settingsService.Save();
        // Apply feature enable status live
        featureHost.SetEnabled("Reminder", EnableReminder);
        featureHost.SetEnabled("HourlyChime", EnableHourlyChime);
        featureHost.SetEnabled("IdleAnimation", EnableIdleAnimation);
    featureHost.SetEnabled("RandomTrick", EnableRandomTrick);
        // Live idle timeout update
        activityMonitor.UpdateIdleThreshold(IdleTimeoutSeconds);
        // Update reminder interval if feature present
        if (featureHost.TryGet("Reminder", out var feature) && feature is Features.ReminderFeature rf)
        {
            rf.UpdateInterval(ReminderIntervalMinutes);
        }
        // Close window
        CloseWindow();
    }

    private void CloseWindow()
    {
        System.Windows.Application.Current.Windows
            .OfType<System.Windows.Window>()
            .FirstOrDefault(w => w is Views.SettingsWindow)?.Close();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
