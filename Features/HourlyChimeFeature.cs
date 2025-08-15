using DigitalPetApp.Helpers;
using DigitalPetApp.Services;

namespace DigitalPetApp.Features
{
    public class HourlyChimeFeature : IAgentFeature
    {
        private readonly INotificationService _notificationService;
        private readonly AgentTimerService _timerService;

        public HourlyChimeFeature(INotificationService notificationService, AgentTimerService timerService)
        {
            _notificationService = notificationService;
            _timerService = timerService;
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
            AnimationHelper.PlayAnimationSequence([Gestures.Greet]);
            _notificationService.ShowNotification($"It's {now:hh tt}! Time for your hourly chime.");
        }
    }
}

