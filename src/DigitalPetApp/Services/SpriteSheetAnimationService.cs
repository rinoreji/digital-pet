using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using DigitalPetApp.Helpers;
using System.Linq;

namespace DigitalPetApp.Services;

public class SpriteSheetAnimationService : IAnimationService
{
    private readonly AgentAnimationLoader animationLoader;
    private readonly RoverSoundLoader soundLoader;
    private readonly System.Windows.Controls.Image imageControl;
    private readonly ISoundPlayerService? soundPlayer;
    private readonly string spriteSheetPath;
    private readonly int frameWidth;
    private readonly int frameHeight;
    private readonly Queue<IReadOnlyList<Gestures>> queue = new();
    private readonly Queue<IReadOnlyList<Gestures>> priorityQueue = new();
    private bool isPlaying = false;
    private readonly object sync = new();
    private BitmapImage? spriteBitmap;
    private readonly int maxQueueLength;

    public SpriteSheetAnimationService(
        AgentAnimationLoader animationLoader,
        RoverSoundLoader soundLoader,
    System.Windows.Controls.Image imageControl,
        string spriteSheetPath,
        int frameWidth,
    int frameHeight,
    ISoundPlayerService? soundPlayer = null,
    int maxQueueLength = 5)
    {
        this.animationLoader = animationLoader;
        this.soundLoader = soundLoader;
        this.imageControl = imageControl;
        this.spriteSheetPath = spriteSheetPath;
        this.frameWidth = frameWidth;
        this.frameHeight = frameHeight;
    this.soundPlayer = soundPlayer;
        this.maxQueueLength = Math.Max(1, maxQueueLength);
    }

    public void PlaySequence(IEnumerable<Gestures> gestures)
    {
        if (gestures == null) return;
    var list = gestures.ToList();
        if (list.Count == 0) return;
        var dispatcher = System.Windows.Application.Current?.Dispatcher;
        if (dispatcher != null && !dispatcher.CheckAccess()) { dispatcher.Invoke(() => PlaySequence(list)); return; }
        lock (sync)
        {
            if (queue.Count >= maxQueueLength)
            {
                // Drop oldest to keep queue responsive.
                queue.Dequeue();
            }
            queue.Enqueue(list);
            if (!isPlaying)
            {
                StartNextLocked();
            }
        }
    }

    public void PlayPriority(IEnumerable<Gestures> gestures, bool interruptCurrent = false)
    {
        if (gestures == null) return;
        var list = gestures.ToList();
        if (list.Count == 0) return;
        var dispatcher = System.Windows.Application.Current?.Dispatcher;
        if (dispatcher != null && !dispatcher.CheckAccess()) { dispatcher.Invoke(() => PlayPriority(list, interruptCurrent)); return; }
        lock (sync)
        {
            // Insert into priority queue (no max length enforcement separate from normal queue; treat both combined for drop policy)
            if ((priorityQueue.Count + queue.Count) >= maxQueueLength)
            {
                // Prefer dropping from normal queue first
                if (queue.Count > 0) queue.Dequeue();
                else if (priorityQueue.Count > 0) priorityQueue.Dequeue();
            }
            priorityQueue.Enqueue(list);
            if (interruptCurrent && isPlaying)
            {
                // Mark playing false; next tick end won't start new one automatically, so we manually start.
                isPlaying = false; // relinquish current
            }
            if (!isPlaying)
            {
                StartNextLocked();
            }
        }
    }

    private void StartNextLocked()
    {
        IReadOnlyList<Gestures>? gestures = null;
        if (priorityQueue.Count > 0)
            gestures = priorityQueue.Dequeue();
        else if (queue.Count > 0)
            gestures = queue.Dequeue();
        if (gestures == null)
        {
            isPlaying = false; return;
        }
        isPlaying = true;
        if (spriteBitmap == null)
        {
            var spriteUri = new Uri(spriteSheetPath, UriKind.Absolute);
            spriteBitmap = new BitmapImage(spriteUri);
        }

        int gestureIndex = 0;
        List<(int x, int y, int duration, string? sound)>? frames = null;
        int frameIndex = 0;
        var timer = new DispatcherTimer();

        void PlayNextGesture()
        {
            if (gestureIndex >= gestures.Count)
            {
                timer.Stop();
                OnSequenceComplete();
                return;
            }
            var gesture = gestures[gestureIndex];
            frames = animationLoader.GetFrames(gesture.ToString());
            frameIndex = 0;
            if (frames == null || frames.Count == 0)
            {
                gestureIndex++;
                PlayNextGesture();
                return;
            }
            timer.Interval = TimeSpan.FromMilliseconds(frames[0].duration);
        }

        timer.Tick += (s, e) =>
        {
            if (frames == null || frames.Count == 0)
            {
                gestureIndex++;
                PlayNextGesture();
                return;
            }
            var (x, y, duration, sound) = frames[frameIndex];
            var rect = new System.Windows.Int32Rect(x, y, frameWidth, frameHeight);
            var cropped = new CroppedBitmap(spriteBitmap!, rect);
            imageControl.Source = cropped;
            if (!string.IsNullOrEmpty(sound))
            {
                var dataUrl = soundLoader.GetSoundDataUrl(sound);
                if (!string.IsNullOrEmpty(dataUrl)) soundPlayer?.PlayDataUrl(dataUrl);
            }
            frameIndex++;
            if (frameIndex >= frames.Count)
            {
                gestureIndex++;
                PlayNextGesture();
            }
            else
            {
                timer.Interval = TimeSpan.FromMilliseconds(frames[frameIndex].duration);
            }
        };

        PlayNextGesture();
        timer.Start();
    }

    private void OnSequenceComplete()
    {
        lock (sync)
        {
            isPlaying = false;
            if (queue.Count > 0)
            {
                StartNextLocked();
            }
        }
    }
}
