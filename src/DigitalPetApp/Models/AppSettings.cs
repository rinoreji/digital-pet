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
    // Audio
    public double Volume { get; set; } = 0.8; // 0..1
    public bool Muted { get; set; } = false;
    // Window position
    public double? WindowLeft { get; set; }
    public double? WindowTop { get; set; }

    // Global hotkey
    public string HotkeyModifier { get; set; } = "Control";
    public string HotkeyKey { get; set; } = "Space";
}
