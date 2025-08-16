using DigitalPetApp.Helpers;
using DigitalPetApp.Services;

namespace DigitalPetApp.Features
{
    public class ReminderFeature : ITogglableFeature
    {
    private readonly INotificationService _notificationService;
    private readonly AgentTimerService _timerService;
    private readonly Services.IAnimationService? _animationService;
    private readonly Services.ActivityMonitor? _activityMonitor;
    private int _intervalMinutes;
        private int _minuteCounter = 0;
        private string _reminderText = "Time for a break!";
    private readonly Services.ILoggingService? _logger;

    public string Key => "Reminder";
    public string DisplayName => "Reminder";
    public bool IsEnabled { get; set; } = true;

    public ReminderFeature(INotificationService notificationService, AgentTimerService timerService, int intervalMinutes = 30, Services.ActivityMonitor? activityMonitor = null, Services.IAnimationService? animationService = null, Services.ILoggingService? logger = null)
        {
            _notificationService = notificationService;
            _timerService = timerService;
            _intervalMinutes = intervalMinutes;
            _activityMonitor = activityMonitor;
            _animationService = animationService;
            _logger = logger;
        }

        public void Initialize()
        {
            // No initialization needed
        }

    public void Start() { if (IsEnabled) _timerService.SecondTick += OnMinuteTick; }
    public void Stop() { _timerService.SecondTick -= OnMinuteTick; }

        public void Update()
        {
            // Could check for conditions or update reminder logic
        }

        private void OnMinuteTick()
        {
            _minuteCounter++;
            if (_minuteCounter >= _intervalMinutes)
            {
                ShowReminder();
                _minuteCounter = 0;
            }
        }

        private void ShowReminder()
        {
            _logger?.Info("Showing reminder notification");
            _notificationService.ShowNotification(_reminderText, "Reminder");
            _animationService?.PlaySequence(new System.Collections.Generic.List<Gestures> { Gestures.GetAttention });
            _activityMonitor?.ReportActivity();
        }

        public void UpdateInterval(int minutes)
        {
            if (minutes <= 0) return;
            _intervalMinutes = minutes;
            _minuteCounter = 0; // reset so new interval counts fresh
            _logger?.Info($"Reminder interval updated to {minutes} minutes");
        }
    }
}
