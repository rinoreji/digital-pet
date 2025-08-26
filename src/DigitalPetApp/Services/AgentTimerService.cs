using System;
using System.Threading;

namespace DigitalPetApp.Services
{
    public class AgentTimerService : IDisposable
    {
    private System.Threading.Timer? _timer;
        private DateTime _lastMinute = DateTime.MinValue;
        private DateTime _lastHour = DateTime.MinValue;

        public event Action? SecondTick;
        public event Action? MinuteTick;
        public event Action? HourTick;

        public AgentTimerService()
        {
            _timer = new System.Threading.Timer(OnTick, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private void OnTick(object? state)
        {
            var now = DateTime.Now;
            SecondTick?.Invoke();

            if (now.Minute != _lastMinute.Minute || now.Hour != _lastMinute.Hour || now.Day != _lastMinute.Day)
            {
                _lastMinute = now;
                MinuteTick?.Invoke();
            }
            if (now.Hour != _lastHour.Hour || now.Day != _lastHour.Day)
            {
                _lastHour = now;
                HourTick?.Invoke();
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
