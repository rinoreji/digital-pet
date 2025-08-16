using System.Collections.ObjectModel;
using System.Windows.Input;
using DigitalPetApp.Services;

namespace DigitalPetApp.ViewModels;

public class MainViewModel
{
    private readonly IAnimationService animationService;
    private readonly INotificationService notificationService;

    public ICommand PetClickedCommand { get; }
    public ICommand ExitCommand { get; }

    public MainViewModel(IAnimationService animationService, INotificationService notificationService)
    {
        this.animationService = animationService;
        this.notificationService = notificationService;
    PetClickedCommand = new RelayCommand(_ => OnPetClicked());
    ExitCommand = new RelayCommand(_ => OnExit());
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
