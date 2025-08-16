using DigitalPetApp.Helpers;
using DigitalPetApp.Services;

namespace DigitalPetApp.Features
{
    public class ReminderFeature : IAgentFeature
    {
    private readonly INotificationService _notificationService;
    private readonly AgentTimerService _timerService;
    private readonly Services.IAnimationService? _animationService;
    private readonly Services.ActivityMonitor? _activityMonitor;
        private readonly int _intervalMinutes;
        private int _minuteCounter = 0;
        private string _reminderText = "Time for a break!";
    private readonly Services.ILoggingService? _logger;

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

        public void Start()
        {
            _timerService.MinuteTick += OnMinuteTick;
        }

        public void Stop()
        {
            _timerService.MinuteTick -= OnMinuteTick;
        }

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
    }
}
