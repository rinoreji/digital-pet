namespace DigitalPetApp.Models;

public class AppSettings
{
    public int IdleTimeoutSeconds { get; set; } = 60;
    public int ReminderIntervalMinutes { get; set; } = 10;
    // Feature toggles
    public bool EnableReminder { get; set; } = true;
    public bool EnableHourlyChime { get; set; } = true;
    public bool EnableIdleAnimation { get; set; } = true;
    public bool EnableRandomTrick { get; set; } = true;
}
