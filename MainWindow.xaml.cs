using System.Text;
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
        // --- UI Configurable constants ---
        private const int WindowWidth = 100;
        private const int WindowHeight = 100;
        private const bool WindowTopmost = true;
        private const bool WindowShowInTaskbar = true;
        private const ResizeMode WindowResizeMode = ResizeMode.NoResize;
        private readonly AgentAnimationLoader animationLoader;
        private readonly RoverSoundLoader soundLoader;
        private readonly FeatureManager featureManager = new FeatureManager();
        private readonly BalloonNotificationService notificationService;

        public MainWindow()
        {
            InitializeComponent();
            this.Topmost = WindowTopmost;
            this.ShowInTaskbar = WindowShowInTaskbar;
            this.ResizeMode = WindowResizeMode;
            this.Width = WindowWidth;
            this.Height = WindowHeight;

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
            AnimationHelper.Init(
                animationLoader,
                soundLoader,
                DogImage,
                "pack://application:,,,/Assets/rover/map.png",
                80, // frame width
                80  // frame height
            );

            // Register features (reminder, notifications, etc.)
            notificationService = new BalloonNotificationService();
            featureManager.RegisterFeature(new ReminderFeature(notificationService));

            // Play default animation on startup (UI only)
            PlayDefaultAnimation();
        }

        private void PlayDefaultAnimation()
        {
            // Example: Play a single animation (can be replaced with user action)
            //Get all Gestures as array
            var allGestures = Enum.GetValues(typeof(Gestures)).Cast<Gestures>().ToArray();
            //AnimationHelper.PlayAnimationSequence(
            //    allGestures
            //);
            AnimationHelper.PlayAnimationSequence(
                 new List<Gestures> { Gestures.Show }
            );

            notificationService.ShowNotification("Rover" + " says hello!", "Dog Clicked");
        }

        private void DogImage_Click(object sender, MouseButtonEventArgs e)
        {
            // Play a fun animation on click
            AnimationHelper.PlayAnimationSequence(
                new List<Gestures> { Gestures.ClickedOn }
            );
            notificationService.ShowNotification("Rover" + " says hello!", "Dog Clicked");
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // AnimationHelper.PlayAnimationSequence(
            //     new List<Gestures> { Gestures.Hide }
            // );            
            // Task.Delay(10000).ContinueWith(_ => Application.Current.Shutdown());
            Application.Current.Shutdown();
        }
    }
}