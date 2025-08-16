using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using DigitalPetApp.Services;

namespace DigitalPetApp.ViewModels;

public class MainViewModel
{
    private readonly IAnimationService animationService;
    private readonly INotificationService notificationService;
    private readonly ISettingsService? settingsService;
    private readonly IFeatureHost? featureHost;

    public ICommand PetClickedCommand { get; }
    public ICommand ExitCommand { get; }
    public ObservableCollection<FeatureToggleViewModel> Features { get; } = new();

    public MainViewModel(IAnimationService animationService, INotificationService notificationService, ISettingsService? settingsService = null, IFeatureHost? featureHost = null)
    {
        this.animationService = animationService;
        this.notificationService = notificationService;
        this.settingsService = settingsService;
        this.featureHost = featureHost;
        PetClickedCommand = new RelayCommand(_ => OnPetClicked());
        ExitCommand = new RelayCommand(_ => OnExit());
        if (featureHost != null)
        {
            foreach (var f in featureHost.GetTogglableFeatures())
            {
                bool initial = f.IsEnabled;
                Features.Add(new FeatureToggleViewModel(f.Key, f.DisplayName, initial, () => ToggleFeature(f.Key)));
            }
        }
    }

    private void OnPetClicked()
    {
        animationService.PlaySequence(new List<Gestures> { Gestures.ClickedOn });
        notificationService.ShowNotification("Rover says hello!", "Dog Clicked");
    }

    public void PlayStartupAnimation()
    {
        animationService.PlaySequence(new List<Gestures> { Gestures.Show });
    }

    private void OnExit()
    {
        System.Windows.Application.Current.Shutdown();
    }

    private void ToggleFeature(string key)
    {
        if (settingsService == null || featureHost == null) return;
        var item = Features.FirstOrDefault(f => f.Key == key);
        if (item == null) return;
        featureHost.SetEnabled(key, item.IsEnabled);
        Debug.WriteLine($"Toggling feature {key} to {item.IsEnabled}");
        switch (key)
        {
            case "Reminder": settingsService.Current.EnableReminder = item.IsEnabled; break;
            case "HourlyChime": settingsService.Current.EnableHourlyChime = item.IsEnabled; break;
            case "IdleAnimation": settingsService.Current.EnableIdleAnimation = item.IsEnabled; break;
        }
        settingsService.Save();
    }
}

public class FeatureToggleViewModel
    : System.ComponentModel.INotifyPropertyChanged
{
    public string Key { get; }
    public string DisplayName { get; }
    private bool isEnabled;
    public bool IsEnabled
    {
        get => isEnabled;
        set
        {
            if (isEnabled == value) return;
            isEnabled = value;
            OnPropertyChanged(nameof(IsEnabled));
        }
    }
    public ICommand ToggleCommand { get; }
    private readonly Action toggleAction;

    public FeatureToggleViewModel(string key, string displayName, bool isEnabled, Action toggleAction)
    {
        Key = key; DisplayName = displayName; this.isEnabled = isEnabled; this.toggleAction = toggleAction;
        ToggleCommand = new RelayCommand(_ => this.toggleAction());
    }

    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
}

public class RelayCommand : ICommand
{
    private readonly Action<object?> execute;
    private readonly Func<object?, bool>? canExecute;

    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        this.execute = execute;
        this.canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => canExecute?.Invoke(parameter) ?? true;
    public void Execute(object? parameter) => execute(parameter);
    public event EventHandler? CanExecuteChanged;
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
