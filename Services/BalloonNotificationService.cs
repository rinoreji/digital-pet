using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace DigitalPetApp.Services
{
    public class BalloonNotificationService : INotificationService
    {
        public void ShowNotification(string message, string title = "Notification")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var balloon = new BalloonNotificationWindow(message, title);
                balloon.Show();
                // Now that the window is shown, ActualWidth/Height are valid
                balloon.Dispatcher.InvokeAsync(() =>
                {
                    var width = balloon.ActualWidth > 0 ? balloon.ActualWidth : balloon.Width;
                    var height = balloon.ActualHeight > 0 ? balloon.ActualHeight : balloon.Height;
                    var mainWindow = Application.Current.MainWindow as Window;
                    if (mainWindow != null && mainWindow.FindName("DogImage") is FrameworkElement dogImage)
                    {
                        // Get DogImage position relative to screen
                        var point = dogImage.PointToScreen(new Point(0, 0));
                        // Get system DPI scaling
                        var source = PresentationSource.FromVisual(mainWindow);
                        double dpiX = 1.0, dpiY = 1.0;
                        if (source != null)
                        {
                            dpiX = source.CompositionTarget.TransformToDevice.M11;
                            dpiY = source.CompositionTarget.TransformToDevice.M22;
                        }
                        // Convert WPF units to physical pixels, then back to WPF units for correct placement
                        double dogImageCenterX = point.X / dpiX + dogImage.ActualWidth / 2;
                        double balloonLeft = dogImageCenterX - width / 2 - 50;
                        double balloonTop = point.Y / dpiY - height; // 10px above head
                        balloon.Left = balloonLeft;
                        balloon.Top = balloonTop;
                    }
                    else
                    {
                        // Fallback: bottom right
                        var desktop = SystemParameters.WorkArea;
                        balloon.Left = desktop.Right - width - 10;
                        balloon.Top = desktop.Bottom - height - 10;
                    }
                }, DispatcherPriority.Loaded);
            });
        }
    }
}
