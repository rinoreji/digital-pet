using System.Diagnostics;
using System.Windows;

namespace DigitalPetApp
{
    public class BalloonNotificationService : INotificationService
    {
        public void ShowNotification(string message, string title = "Notification")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var balloon = new BalloonNotificationWindow(message, title);
                Debug.WriteLine(message + title);
                balloon.Show();
                // Now that the window is shown, ActualWidth/Height are valid
                balloon.Dispatcher.InvokeAsync(() =>
                {
                    var width = balloon.ActualWidth > 0 ? balloon.ActualWidth : balloon.Width;
                    var height = balloon.ActualHeight > 0 ? balloon.ActualHeight : balloon.Height;
                    var mainWindow = Application.Current.MainWindow as Window;
                    if (mainWindow != null && mainWindow.FindName("DogImage") is System.Windows.FrameworkElement dogImage)
                    {
                        // Get DogImage position relative to screen
                        var point = dogImage.PointToScreen(new System.Windows.Point(0, 0));
                        // Get system DPI scaling
                        var source = System.Windows.PresentationSource.FromVisual(mainWindow);
                        double dpiX = 1.0, dpiY = 1.0;
                        if (source != null)
                        {
                            dpiX = source.CompositionTarget.TransformToDevice.M11;
                            dpiY = source.CompositionTarget.TransformToDevice.M22;
                        }
                        // Convert WPF units to physical pixels, then back to WPF units for correct placement
                        double dogImageCenterX = (point.X / dpiX) + (dogImage.ActualWidth / 2);
                        double balloonLeft = dogImageCenterX - (width / 2) - 50;
                        double balloonTop = (point.Y / dpiY) - height; // 10px above head
                        balloon.Left = balloonLeft;
                        balloon.Top = balloonTop;
                        Debug.WriteLine($"point:{point}, left:{balloonLeft}, dog:{dogImage.ActualWidth}, baloon:{width}, hardcoded:{balloon.Left}");
                    }
                    else
                    {
                        // Fallback: bottom right
                        var desktop = SystemParameters.WorkArea;
                        balloon.Left = desktop.Right - width - 10;
                        balloon.Top = desktop.Bottom - height - 10;
                    }
                }, System.Windows.Threading.DispatcherPriority.Loaded);
            });
        }
    }
}
