using System;
using System.Collections.Generic;
using DigitalPetApp.Services;
using DigitalPetApp.Helpers;
using System.Diagnostics;
using static System.Reflection.Metadata.BlobBuilder;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Windows;

namespace DigitalPetApp.Features
{
    public class IdleAnimationFeature : ITogglableFeature
    {
    private readonly ActivityMonitor _activityMonitor;
    private readonly AgentTimerService _timerService;
    private readonly Services.IAnimationService? _animationService;
    private readonly Services.ILoggingService? _logger;
        private static readonly Random _rng = new();
        // Small set of candidate gestures to use when idle happens
        private static readonly Gestures[] _idleGesturePool = new[]
        {
            Gestures.RestPose,
            Gestures.GetAttention,
            Gestures.Pleased,
            Gestures.Surprised,
            Gestures.Congratulate,
            //Gestures.Hide,
            Gestures.Acknowledge,
            Gestures.Thinking,
            Gestures.Travel,
            Gestures.Cooking,
            Gestures.Writing,
            Gestures.GetAttention,
            Gestures.GestureLeft,
            Gestures.Surprised,
            Gestures.Shopping,
            Gestures.ImageSearching,
            Gestures.Celebrity,
            Gestures.LookUpLeft,
            Gestures.Greet,
            //Gestures.Idle,
            //Gestures.HideQuick,
            Gestures.CharacterSucceeds,
            Gestures.Sports,
            Gestures.Show,
            Gestures.Money,
            Gestures.Searching,
            Gestures.Embarrassed,
            Gestures.Books,
            Gestures.LookUp,
            Gestures.ClickedOn,
            //Gestures.GetAttentionMinor,
            Gestures.RestPose,
            Gestures.Pleased
        };

    public string Key => "IdleAnimation";
    public string DisplayName => "Idle Animation";
    public bool IsEnabled { get; set; } = true;

    public IdleAnimationFeature(ActivityMonitor activityMonitor, AgentTimerService timerService, Services.IAnimationService? animationService = null, Services.ILoggingService? logger = null)
        {
            _activityMonitor = activityMonitor ?? throw new ArgumentNullException(nameof(activityMonitor));
            _timerService = timerService ?? throw new ArgumentNullException(nameof(timerService));
            _animationService = animationService;
            _logger = logger;
        }

        public void Initialize()
        {
            // No-op
        }

        public void Start()
        {
            if (!IsEnabled) return;
            _activityMonitor.IdleStarted += OnIdleStarted;
            _activityMonitor.IdleEnded += OnIdleEnded;
            // If feature is re-enabled while the system is already idle, fire immediately
            if (_activityMonitor.IsIdle)
            {
                OnIdleStarted();
            }
        }

        public void Stop()
        {
            _activityMonitor.IdleStarted -= OnIdleStarted;
            _activityMonitor.IdleEnded -= OnIdleEnded;
        }

        public void Update()
        {
            // Nothing periodic required; ActivityMonitor uses shared timer
        }

        private void OnIdleStarted()
        {
            _logger?.Info("Idle started");
            // Play 1-2 random idle gestures from the pool
            try
            {
                int take = 1 + (_rng.Next(0, 2)); // 1 or 2
                var chosen = new List<Gestures>(take);
                lock (_rng)
                {
                    // Select without replacement
                    var indices = new List<int>();
                    for (int i = 0; i < _idleGesturePool.Length; i++) indices.Add(i);
                    for (int i = 0; i < take && indices.Count > 0; i++)
                    {
                        int idx = indices[_rng.Next(indices.Count)];
                        chosen.Add(_idleGesturePool[idx]);
                        indices.Remove(idx);
                    }
                }
                Debug.WriteLine($"   ============>   Playing idle gestures: {string.Join(", ", chosen)}");
                if (chosen.Count > 0)
                    _animationService?.PlaySequence(chosen);
                _activityMonitor.ReportActivity(); // Report activity to reset idle timer
            }
            catch
            {
                // Swallow errors to avoid crashing the timer thread or feature manager
            }
        }

        private void OnIdleEnded()
        {
            // Optionally play a small resume animation
            try
            {
                //AnimationHelper.PlayAnimationSequence(new List<Gestures> { Gestures.Pleased });
            }
            catch { }
        }
    }
}
