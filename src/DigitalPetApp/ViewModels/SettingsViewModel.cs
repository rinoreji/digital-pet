using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DigitalPetApp.Services;

namespace DigitalPetApp.ViewModels;

public class SettingsViewModel : INotifyPropertyChanged
{
    public IEnumerable<string> AvailableModifiers { get; } = Enum.GetNames(typeof(System.Windows.Input.ModifierKeys)).Where(m => m != "None");
    public IEnumerable<string> AvailableKeys { get; } =
        new[] { "Space" }
        .Concat(Enumerable.Range('A', 26).Select(i => ((char)i).ToString()));
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
        volume = settingsService.Current.Volume;
        muted = settingsService.Current.Muted;
        hotkeyModifier = settingsService.Current.HotkeyModifier;
        hotkeyKey = settingsService.Current.HotkeyKey;
        SaveCommand = new RelayCommand(_ => Save());
        CancelCommand = new RelayCommand(_ => CloseWindow());
    }
    private string hotkeyModifier;
    public string HotkeyModifier { get => hotkeyModifier; set { if (value != hotkeyModifier) { hotkeyModifier = value; OnPropertyChanged(); } } }

    private string hotkeyKey;
    public string HotkeyKey { get => hotkeyKey; set { if (value != hotkeyKey) { hotkeyKey = value; OnPropertyChanged(); } } }

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

    private double volume;
    public double Volume { get => volume; set { var v = Math.Clamp(value,0,1); if (Math.Abs(v-volume) > 0.0001){ volume=v; OnPropertyChanged(); } } }

    private bool muted;
    public bool Muted { get => muted; set { if (value!=muted){ muted=value; OnPropertyChanged(); } } }

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
    settingsService.Current.Volume = Volume;
    settingsService.Current.Muted = Muted;
    settingsService.Current.HotkeyModifier = HotkeyModifier;
    settingsService.Current.HotkeyKey = HotkeyKey;
    settingsService.Save();
        // Apply audio settings live if service available
        try
        {
            var sp = Services.ServiceRegistry.Resolve<Services.ISoundPlayerService>();
            sp.Volume = Volume;
            sp.Muted = Muted;
        }
        catch { }
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
