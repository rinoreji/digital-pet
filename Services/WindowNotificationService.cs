using System.Windows;

namespace DigitalPetApp.Services
{
    public class WindowNotificationService : INotificationService
    {
        public void ShowNotification(string message, string title = "Notification")
        {
            System.Windows.MessageBox.Show(message, title, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }
}
