namespace DigitalPetApp.Models;

public class AppSettings
{
    public int IdleTimeoutSeconds { get; set; } = 60;
    public int ReminderIntervalMinutes { get; set; } = 30;
}
