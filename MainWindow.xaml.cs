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
        // --- Configurable constants ---
        private const string SpriteSheetPath = "pack://application:,,,/Assets/rover/map.png";
        private const int SpriteFrameWidth = 80;
        private const int SpriteFrameHeight = 80;
        private const int WindowWidth = 100;
        private const int WindowHeight = 100;
        private const bool WindowTopmost = true;
        private const bool WindowShowInTaskbar = true;
        private const ResizeMode WindowResizeMode = ResizeMode.NoResize;
    // Animation loader for agent.json
        private static string GetAgentJsonPath()
        {
            var exeDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            return System.IO.Path.Combine(exeDir ?? "", "Assets", "rover", "agent.json");
        }
    private static readonly string AnimationName = "Acknowledge";
    private readonly List<(int x, int y, int duration, string? sound)> frames;

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

            // Load animation frames from agent.json
            var loader = new AgentAnimationLoader(GetAgentJsonPath());
            frames = loader.GetFrames(AnimationName);
            if (frames == null || frames.Count == 0)
            {
                MessageBox.Show($"Animation '{AnimationName}' not found in agent.json.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                frames = new List<(int x, int y, int duration, string? sound)> { (0, 0, 100, null) };
            }

            int frameWidth = SpriteFrameWidth;
            int frameHeight = SpriteFrameHeight;
            int currentFrame = 0;
            var spriteUri = new System.Uri(SpriteSheetPath, System.UriKind.Absolute);
            var spriteBitmap = new BitmapImage(spriteUri);

            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = System.TimeSpan.FromMilliseconds(frames[0].duration);
            timer.Tick += (s2, e2) =>
            {
                var (x, y, duration, sound) = frames[currentFrame];
                var rect = new Int32Rect(x, y, frameWidth, frameHeight);
                var cropped = new CroppedBitmap(spriteBitmap, rect);
                DogImage.Source = cropped;
                // Optionally: play sound here using 'sound' if not null
                currentFrame = (currentFrame + 1) % frames.Count;
                timer.Interval = System.TimeSpan.FromMilliseconds(frames[currentFrame].duration);
            };
            timer.Start();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}