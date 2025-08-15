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
                // Position bottom right, above taskbar
                var desktop = SystemParameters.WorkArea;
                balloon.Left = desktop.Right - balloon.Width - 10;
                balloon.Top = desktop.Bottom - balloon.Height - 10;
                balloon.Show();
            });
        }
    }
}
