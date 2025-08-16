using System;
using System.Collections.Generic;
using DigitalPetApp.Helpers;
using DigitalPetApp.Services;

namespace DigitalPetApp.Features
{
    public class HourlyChimeFeature : ITogglableFeature
    {
    private readonly INotificationService _notificationService;
    private readonly AgentTimerService _timerService;
    private readonly ActivityMonitor? _activityMonitor;
    private readonly Services.IAnimationService? _animationService;
    private readonly Services.ILoggingService? _logger;

    public string Key => "HourlyChime";
    public string DisplayName => "Hourly Chime";
    public bool IsEnabled { get; set; } = true;

    public HourlyChimeFeature(INotificationService notificationService, AgentTimerService timerService, ActivityMonitor? activityMonitor = null, Services.IAnimationService? animationService = null, Services.ILoggingService? logger = null)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _timerService = timerService ?? throw new ArgumentNullException(nameof(timerService));
            _activityMonitor = activityMonitor;
            _animationService = animationService;
            _logger = logger;
        }

        public void Initialize()
        {
            // No initialization needed for this feature
        }

    public void Start() { if (IsEnabled) _timerService.HourTick += OnHourTick; }
    public void Stop() { _timerService.HourTick -= OnHourTick; }

        public void Update()
        {
            // No periodic update needed
        }

        private void OnHourTick()
        {
            var now = DateTime.Now;
            _logger?.Info($"Hourly chime at {now:HH:mm}");
            _animationService?.PlaySequence(new List<Gestures> { Gestures.Greet });
            _notificationService.ShowNotification($"It's {now:hh tt}!");
            _activityMonitor?.ReportActivity();
        }
    }
}

