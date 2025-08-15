namespace DigitalPetApp
{
    public interface INotificationService
    {
        void ShowNotification(string message, string title = "Notification");
    }
}
