using DigitalPetApp.Services;
using System;
using System.Windows;
using System.Windows.Threading;

namespace DigitalPetApp.Features
{
    public class ReminderFeature : IAgentFeature
    {
    private INotificationService? _notificationService;
        private DispatcherTimer? _timer;
        private TimeSpan _interval = TimeSpan.FromSeconds(10); // Default: 1 hour
        private string _reminderText = "Time for a break!";

        public ReminderFeature(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void Initialize()
        {
            _timer = new DispatcherTimer { Interval = _interval };
            _timer.Tick += (s, e) => ShowReminder();
            _timer.Start();
        }

        public void Update()
        {
            // Could check for conditions or update reminder logic
        }

        private void ShowReminder()
        {
            _notificationService?.ShowNotification(_reminderText, "Reminder");
            // Optionally trigger an animation or sound here
        }
    }
}
