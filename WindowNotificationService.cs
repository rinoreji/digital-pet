using System.Windows;

namespace DigitalPetApp
{
    public class WindowNotificationService : INotificationService
    {
        public void ShowNotification(string message, string title = "Notification")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
