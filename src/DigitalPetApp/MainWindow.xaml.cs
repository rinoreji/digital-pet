using DigitalPetApp.Features;
using DigitalPetApp.Helpers;
using DigitalPetApp.Services;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace DigitalPetApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
    private readonly Services.FeatureHost featureHost = new Services.FeatureHost();
        private readonly BalloonNotificationService notificationService;
        private readonly AgentTimerService timerService = new AgentTimerService();
        private readonly ActivityMonitor? activityMonitor;
    private readonly ViewModels.MainViewModel viewModel;
    private readonly SettingsService settingsService = new SettingsService();
        private readonly AgentAnimationLoader animationLoader;
    private readonly RoverSoundLoader soundLoader;
    private readonly PooledSoundPlayerService soundPlayer;
    private readonly TrayIconService trayIcon;

        public MainWindow()
        {
            InitializeComponent();
            this.Topmost = Services.UISettings.WindowTopmost;
            this.ShowInTaskbar = Services.UISettings.WindowShowInTaskbar;
            this.ResizeMode = ResizeMode.NoResize;
            this.Width = Services.UISettings.WindowWidth;
            this.Height = Services.UISettings.WindowHeight;

            // Position window using stored settings or default bottom-right
            if (settingsService.Current.WindowLeft.HasValue && settingsService.Current.WindowTop.HasValue)
            {
                this.Left = settingsService.Current.WindowLeft.Value;
                this.Top = settingsService.Current.WindowTop.Value;
            }
            else
            {
                var desktopWorkingArea = SystemParameters.WorkArea;
                this.Left = desktopWorkingArea.Right - this.Width;
                this.Top = desktopWorkingArea.Bottom - this.Height;
            }

            // Initialize animation and sound loaders
            var exeDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var agentJsonPath = System.IO.Path.Combine(exeDir ?? "", "Assets", "rover", "agent.json");
            var soundJsonPath = System.IO.Path.Combine(exeDir ?? "", "Assets", "rover", "sounds-mp3.json");
            animationLoader = new AgentAnimationLoader(agentJsonPath);
            soundLoader = new RoverSoundLoader(soundJsonPath);

            // Animation service initialized (legacy AnimationHelper removed)
            soundPlayer = new PooledSoundPlayerService();
            soundPlayer.Volume = settingsService.Current.Volume;
            soundPlayer.Muted = settingsService.Current.Muted;
            var animationService = new Services.SpriteSheetAnimationService(
                animationLoader,
                soundLoader,
                DogImage,
                "pack://application:,,,/Assets/rover/map.png",
                Services.UISettings.SpriteFrameWidth,
                Services.UISettings.SpriteFrameHeight,
                soundPlayer);
            var logger = new FileLoggingService();
            Services.ServiceRegistry.Register<Services.IAnimationService>(animationService);
            Services.ServiceRegistry.Register<Services.ISoundPlayerService>(soundPlayer);
            Services.ServiceRegistry.Register<Services.INotificationService>(notificationService = new BalloonNotificationService());
            Services.ServiceRegistry.Register<Services.ILoggingService>(logger);

            // Create ActivityMonitor using shared timer; idle threshold from settings
            var monitor = new ActivityMonitor(timerService, idleSeconds: settingsService.Current.IdleTimeoutSeconds);
            this.activityMonitor = monitor;

            // Register features before creating ViewModel so it can enumerate them
            var reminder = new ReminderFeature(notificationService, timerService, intervalMinutes: settingsService.Current.ReminderIntervalMinutes, activityMonitor: monitor, animationService: animationService, logger: logger) { IsEnabled = settingsService.Current.EnableReminder };
            var chime = new HourlyChimeFeature(notificationService, timerService, monitor, animationService, logger) { IsEnabled = settingsService.Current.EnableHourlyChime };
            var idle = new IdleAnimationFeature(monitor, timerService, animationService, logger) { IsEnabled = settingsService.Current.EnableIdleAnimation };
            var randomTrick = new RandomTrickFeature(timerService, monitor, animationService, logger) { IsEnabled = settingsService.Current.EnableRandomTrick };
            featureHost.RegisterFeature("Reminder", reminder, start: reminder.IsEnabled);
            featureHost.RegisterFeature("HourlyChime", chime, start: chime.IsEnabled);
            featureHost.RegisterFeature("IdleAnimation", idle, start: idle.IsEnabled);
            featureHost.RegisterFeature("RandomTrick", randomTrick, start: randomTrick.IsEnabled);

            // Now create ViewModel which will populate Features collection
            viewModel = new ViewModels.MainViewModel(animationService, notificationService, settingsService, featureHost, monitor);
            this.DataContext = viewModel;

            // Tray icon
            trayIcon = new TrayIconService(this, () => viewModel.OpenSettingsCommand.Execute(null));
            trayIcon.Show();

            // Play default animation on startup (UI only)
            viewModel.PlayStartupAnimation();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            timerService.Dispose();
            activityMonitor?.Dispose();
            trayIcon.Dispose();
            // Persist changes only if dirty (already tracked)
            // Persist window position and audio settings (latest in case adjusted elsewhere)
            settingsService.Current.WindowLeft = this.Left;
            settingsService.Current.WindowTop = this.Top;
            settingsService.Current.Volume = soundPlayer.Volume;
            settingsService.Current.Muted = soundPlayer.Muted;
            settingsService.SaveIfDirty();
        }

        // Simplified drag-or-click (single handler)
        private DateTime _mouseDownTime;
        private System.Windows.Point _mouseDownPos;
        private const double DragThreshold = 6; // px
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _mouseDownTime = DateTime.Now;
            _mouseDownPos = e.GetPosition(this);
            var wasLeft = this.Left; var wasTop = this.Top;
            try { this.DragMove(); } catch { }
            // If position changed, save & treat as drag
            if (Math.Abs(this.Left - wasLeft) > 0.5 || Math.Abs(this.Top - wasTop) > 0.5)
            {
                settingsService.Current.WindowLeft = this.Left;
                settingsService.Current.WindowTop = this.Top;
                settingsService.SaveIfDirty();
                return; // drag consumed
            }
            // Otherwise treat as click (quick & small movement)
            if (viewModel.PetClickedCommand.CanExecute(null)) viewModel.PetClickedCommand.Execute(null);
        }

    }
}