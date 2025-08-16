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
        private readonly AgentAnimationLoader animationLoader;
        private readonly RoverSoundLoader soundLoader;

        public MainWindow()
        {
            InitializeComponent();
            this.Topmost = Services.UISettings.WindowTopmost;
            this.ShowInTaskbar = Services.UISettings.WindowShowInTaskbar;
            this.ResizeMode = ResizeMode.NoResize;
            this.Width = Services.UISettings.WindowWidth;
            this.Height = Services.UISettings.WindowHeight;

            // Position window above taskbar (bottom right)
            var desktopWorkingArea = SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;

            // Initialize animation and sound loaders
            var exeDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var agentJsonPath = System.IO.Path.Combine(exeDir ?? "", "Assets", "rover", "agent.json");
            var soundJsonPath = System.IO.Path.Combine(exeDir ?? "", "Assets", "rover", "sounds-mp3.json");
            animationLoader = new AgentAnimationLoader(agentJsonPath);
            soundLoader = new RoverSoundLoader(soundJsonPath);

            // Initialize AnimationHelper with one-time parameters
            var animationService = new Services.SpriteSheetAnimationService(
                animationLoader,
                soundLoader,
                DogImage,
                "pack://application:,,,/Assets/rover/map.png",
                Services.UISettings.SpriteFrameWidth,
                Services.UISettings.SpriteFrameHeight);
            var logger = new FileLoggingService();
            Services.ServiceRegistry.Register<Services.IAnimationService>(animationService);
            Services.ServiceRegistry.Register<Services.INotificationService>(notificationService = new BalloonNotificationService());
            Services.ServiceRegistry.Register<Services.ILoggingService>(logger);
            viewModel = new ViewModels.MainViewModel(animationService, notificationService);
            this.DataContext = viewModel;

            // Register features (reminder, notifications, idle animation, etc.)
            // Create ActivityMonitor using shared timer; idle threshold 60s (configurable)
            var monitor = new ActivityMonitor(timerService, idleSeconds: 60);
            this.activityMonitor = monitor;

            // Register features, passing the activity monitor so they can report activity
            featureHost.RegisterFeature(new ReminderFeature(notificationService, timerService, intervalMinutes: 30, activityMonitor: monitor, animationService: animationService, logger: logger));
            featureHost.RegisterFeature(new HourlyChimeFeature(notificationService, timerService, monitor, animationService, logger));
            featureHost.RegisterFeature(new IdleAnimationFeature(monitor, timerService, animationService, logger));

            // Play default animation on startup (UI only)
            viewModel.PlayStartupAnimation();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            timerService.Dispose();
            activityMonitor?.Dispose();
        }

    // Event handlers removed in favor of command bindings.
    }
}