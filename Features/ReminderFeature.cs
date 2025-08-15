using DigitalPetApp.Helpers;
using DigitalPetApp.Services;

namespace DigitalPetApp.Features
{
    public class ReminderFeature : IAgentFeature
    {
        private readonly INotificationService _notificationService;
        private readonly AgentTimerService _timerService;
        private readonly int _intervalMinutes;
        private int _minuteCounter = 0;
        private string _reminderText = "Time for a break!";

        public ReminderFeature(INotificationService notificationService, AgentTimerService timerService, int intervalMinutes = 30)
        {
            _notificationService = notificationService;
            _timerService = timerService;
            _intervalMinutes = intervalMinutes;
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
            _notificationService.ShowNotification(_reminderText, "Reminder");
            AnimationHelper.PlayAnimationSequence([Gestures.GetAttention]);
        }
    }
}
