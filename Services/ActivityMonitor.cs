using System;
using System.Diagnostics;

namespace DigitalPetApp.Services
{
    public class ActivityMonitor : IDisposable
    {
        private readonly AgentTimerService _timerService;
        private DateTime _lastActivity;
        private bool _isIdle = false;
        private readonly object _lock = new();

        public event Action? IdleStarted;
        public event Action? IdleEnded;

        public TimeSpan IdleThreshold { get; }

        public ActivityMonitor(AgentTimerService timerService, int idleSeconds = 180)
        {
            _timerService = timerService ?? throw new ArgumentNullException(nameof(timerService));
            IdleThreshold = TimeSpan.FromSeconds(idleSeconds);
            _lastActivity = DateTime.Now;

            // Listen to the shared timer so we can evaluate idle state regularly
            _timerService.SecondTick += OnSecondTick;
        }

        public void ReportActivity()
        {
            Debug.WriteLine("    ====>  Activity reported at " + DateTime.Now);
            lock (_lock)
            {
                _lastActivity = DateTime.Now;
                if (_isIdle)
                {
                    _isIdle = false;
                    IdleEnded?.Invoke();
                }
            }
        }

        public bool IsIdle
        {
            get
            {
                lock (_lock) { return _isIdle; }
            }
        }

        private void OnSecondTick()
        {
            lock (_lock)
            {
                if (!_isIdle && (DateTime.Now - _lastActivity) >= IdleThreshold)
                {
                    _isIdle = true;
                    IdleStarted?.Invoke();
                }
            }
        }

        public void Dispose()
        {
            _timerService.SecondTick -= OnSecondTick;
        }
    }
}
