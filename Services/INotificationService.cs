namespace DigitalPetApp.Services
{
    public interface INotificationService
    {
        void ShowNotification(string message, string title = "Notification");
    void ShowPriorityNotification(string message, string title = "Notification");
    }
}
