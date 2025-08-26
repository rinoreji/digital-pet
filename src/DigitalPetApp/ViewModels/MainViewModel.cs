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
    public ICommand OpenSettingsCommand { get; }
    private readonly ActivityMonitor? activityMonitor;

    public MainViewModel(IAnimationService animationService, INotificationService notificationService, ISettingsService? settingsService = null, IFeatureHost? featureHost = null, ActivityMonitor? activityMonitor = null)
    {
        this.animationService = animationService;
        this.notificationService = notificationService;
        this.settingsService = settingsService;
        this.featureHost = featureHost;
        this.activityMonitor = activityMonitor;
    PetClickedCommand = new RelayCommand(_ => OnPetClicked());
    ExitCommand = new RelayCommand(_ => OnExit());
    OpenSettingsCommand = new RelayCommand(_ => OnOpenSettings());
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

    private void OnOpenSettings()
    {
        if (settingsService == null || featureHost == null) return;
        // Find existing
        var existing = System.Windows.Application.Current.Windows.OfType<System.Windows.Window>().FirstOrDefault(w => w is Views.SettingsWindow);
        if (existing != null) { existing.Activate(); return; }
        var mainWin = System.Windows.Application.Current.MainWindow;
    var vm = new SettingsViewModel(settingsService, featureHost, activityMonitor!);
        var win = new Views.SettingsWindow { DataContext = vm, Owner = mainWin };
        win.Show();
    }

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
