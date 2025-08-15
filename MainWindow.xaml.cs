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
        public MainWindow()
        {
            InitializeComponent();
            // Make window borderless, always on top, not shown in taskbar
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;
            this.Background = Brushes.LightGray; // For visibility during debugging
            this.Topmost = true;
            this.ShowInTaskbar = true;
            this.ResizeMode = ResizeMode.NoResize;

            // Position window above taskbar (bottom left)
            var desktopWorkingArea = SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Left;
            this.Top = desktopWorkingArea.Bottom - this.Height;
            this.Width = 150;
            this.Height = 150;

            // Animate the window itself along the taskbar
            this.Loaded += (s, e) => {
                var windowAnimation = new System.Windows.Media.Animation.DoubleAnimation
                {
                    From = desktopWorkingArea.Left,
                    To = desktopWorkingArea.Right - this.Width,
                    Duration = new Duration(System.TimeSpan.FromSeconds(5)),
                    AutoReverse = true,
                    RepeatBehavior = System.Windows.Media.Animation.RepeatBehavior.Forever
                };
                this.BeginAnimation(Window.LeftProperty, windowAnimation);
            };
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}