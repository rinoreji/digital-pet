using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace DigitalPetApp.Services
{
    public class BalloonNotificationService : INotificationService
    {
        private readonly IBalloonPositionService positionService;
    private BalloonNotificationWindow? activeBalloon;
        public BalloonNotificationService() : this(new BalloonPositionService()) { }
        public BalloonNotificationService(IBalloonPositionService positionService)
        {
            this.positionService = positionService;
        }

        public void ShowNotification(string message, string title = "Notification")
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                var balloon = new BalloonNotificationWindow(message, title);
                ShowBalloon(balloon);
            });
        }

        public void ShowPriorityNotification(string message, string title = "Notification")
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                // Close any existing balloon immediately
                if (activeBalloon != null)
                {
                    try { activeBalloon.Close(); } catch { }
                    activeBalloon = null;
                }
                var balloon = new BalloonNotificationWindow(message, title + " !");
                // Optional: could style differently here (e.g., bold title) by updating window if desired later.
                ShowBalloon(balloon, isPriority: true);
            });
        }

        private void ShowBalloon(BalloonNotificationWindow balloon, bool isPriority = false)
        {
            activeBalloon = balloon;
            balloon.Show();
            balloon.Dispatcher.InvokeAsync(() =>
            {
                var mainWindow = System.Windows.Application.Current.MainWindow as System.Windows.Window;
                FrameworkElement? anchor = null;
                if (mainWindow != null)
                {
                    anchor = mainWindow.FindName("DogImage") as FrameworkElement;
                }
                var (left, top) = positionService.GetBalloonPosition(balloon, anchor);
                balloon.Left = left;
                balloon.Top = top;
            }, DispatcherPriority.Loaded);
        }
    }
}
