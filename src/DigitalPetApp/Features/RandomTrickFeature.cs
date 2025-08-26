using System;
using System.Collections.Generic;
using DigitalPetApp.Helpers;
using DigitalPetApp.Services;

namespace DigitalPetApp.Features;

public class RandomTrickFeature : ITogglableFeature
{
    private readonly AgentTimerService timerService;
    private readonly ActivityMonitor activityMonitor;
    private readonly IAnimationService animationService;
    private readonly ILoggingService? logger;
    private readonly Random rng = new();
    private int secondsUntilNext;
    private readonly int minIntervalSeconds;
    private readonly int maxIntervalSeconds;
    private int secondsSinceLastActivity;
    private readonly Gestures[] trickPool = new[]
    {
        Gestures.Travel,
        Gestures.Cooking,
        Gestures.Writing,
        Gestures.Sports,
        Gestures.Celebrity,
        Gestures.Money,
        Gestures.ImageSearching,
        Gestures.Searching,
        Gestures.Books,
        Gestures.Thinking,
        Gestures.Greet,
        Gestures.CharacterSucceeds
    };

    public string Key => "RandomTrick";
    public string DisplayName => "Random Trick";
    public bool IsEnabled { get; set; } = true;

    public RandomTrickFeature(AgentTimerService timerService, ActivityMonitor activityMonitor, IAnimationService animationService, ILoggingService? logger = null, int minIntervalSeconds = 90, int maxIntervalSeconds = 300)
    {
        this.timerService = timerService;
        this.activityMonitor = activityMonitor;
        this.animationService = animationService;
        this.logger = logger;
        this.minIntervalSeconds = Math.Max(10, minIntervalSeconds);
        this.maxIntervalSeconds = Math.Max(this.minIntervalSeconds + 1, maxIntervalSeconds);
    }

    public void Initialize() { }

    public void Start()
    {
        if (!IsEnabled) return;
        timerService.SecondTick += OnSecond;
        activityMonitor.IdleStarted += OnIdleStarted;
        activityMonitor.IdleEnded += OnIdleEnded;
        ResetCountdown();
    }

    public void Stop()
    {
        timerService.SecondTick -= OnSecond;
        activityMonitor.IdleStarted -= OnIdleStarted;
        activityMonitor.IdleEnded -= OnIdleEnded;
    }

    public void Update() { }

    private void OnSecond()
    {
        if (!IsEnabled) return;
        secondsSinceLastActivity++;
        if (secondsUntilNext > 0)
        {
            secondsUntilNext--;
            return;
        }
        if (secondsSinceLastActivity < 15) return; // need some idle time
        PlayRandomTrick();
        ResetCountdown();
    }

    private void PlayRandomTrick()
    {
        try
        {
            var gesture = trickPool[rng.Next(trickPool.Length)];
            logger?.Info($"Random trick: {gesture}");
            animationService.PlaySequence(new List<Gestures> { gesture });
            activityMonitor.ReportActivity();
            secondsSinceLastActivity = 0;
        }
        catch { }
    }

    private void ResetCountdown()
    {
        secondsUntilNext = rng.Next(minIntervalSeconds, maxIntervalSeconds + 1);
        logger?.Info($"Next trick in {secondsUntilNext}s");
    }

    private void OnIdleStarted()
    {
        if (secondsUntilNext > 10) secondsUntilNext = 10; // speed up when idle
    }

    private void OnIdleEnded()
    {
        secondsSinceLastActivity = 0; // wait again after activity resumes
    }
}
