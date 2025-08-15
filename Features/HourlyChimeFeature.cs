using System;
using System.Collections.Generic;
using DigitalPetApp.Helpers;
using DigitalPetApp.Services;

namespace DigitalPetApp.Features
{
    public class HourlyChimeFeature : IAgentFeature
    {
        private readonly INotificationService _notificationService;
        private readonly AgentTimerService _timerService;
        private readonly ActivityMonitor? _activityMonitor;

        public HourlyChimeFeature(INotificationService notificationService, AgentTimerService timerService, ActivityMonitor? activityMonitor = null)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _timerService = timerService ?? throw new ArgumentNullException(nameof(timerService));
            _activityMonitor = activityMonitor;
        }

        public void Initialize()
        {
            // No initialization needed for this feature
        }

        public void Start()
        {
            _timerService.HourTick += OnHourTick;
        }

        public void Stop()
        {
            _timerService.HourTick -= OnHourTick;
        }

        public void Update()
        {
            // No periodic update needed
        }

        private void OnHourTick()
        {
            var now = DateTime.Now;
            AnimationHelper.PlayAnimationSequence(new List<Gestures> { Gestures.Greet });
            _notificationService.ShowNotification($"It's {now:hh tt}!");
            _activityMonitor?.ReportActivity();
        }
    }
}

