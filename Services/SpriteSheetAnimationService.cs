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
    private readonly Image imageControl;
    private readonly ISoundPlayerService? soundPlayer;
    private readonly string spriteSheetPath;
    private readonly int frameWidth;
    private readonly int frameHeight;

    public SpriteSheetAnimationService(
        AgentAnimationLoader animationLoader,
        RoverSoundLoader soundLoader,
        Image imageControl,
        string spriteSheetPath,
        int frameWidth,
    int frameHeight,
    ISoundPlayerService? soundPlayer = null)
    {
        this.animationLoader = animationLoader;
        this.soundLoader = soundLoader;
        this.imageControl = imageControl;
        this.spriteSheetPath = spriteSheetPath;
        this.frameWidth = frameWidth;
        this.frameHeight = frameHeight;
    this.soundPlayer = soundPlayer;
    }

    public void PlaySequence(IEnumerable<Gestures> gestures)
    {
        if (gestures == null || !gestures.Any()) return;
        var dispatcher = System.Windows.Application.Current?.Dispatcher;
        if (dispatcher != null && !dispatcher.CheckAccess())
        {
            dispatcher.Invoke(() => PlaySequence(gestures));
            return;
        }

        var spriteUri = new Uri(spriteSheetPath, UriKind.Absolute);
        var spriteBitmap = new BitmapImage(spriteUri);
        int gestureAnimIndex = 0;
        List<(int x, int y, int duration, string? sound)>? frames = null;
        int currentFrame = 0;
        DispatcherTimer timer = new DispatcherTimer();

        void PlayNextAnimation()
        {
            if (gestureAnimIndex >= gestures.Count())
            {
                timer.Stop();
                return;
            }
            var gesture = gestures.ElementAt(gestureAnimIndex);
            frames = animationLoader.GetFrames(gesture.ToString());
            currentFrame = 0;
            if (frames == null || frames.Count == 0)
            {
                gestureAnimIndex++;
                PlayNextAnimation();
                return;
            }
            timer.Interval = TimeSpan.FromMilliseconds(frames[0].duration);
        }

        timer.Tick += (s, e) =>
        {
            if (frames == null || frames.Count == 0)
            {
                gestureAnimIndex++;
                PlayNextAnimation();
                return;
            }
            var (x, y, duration, sound) = frames[currentFrame];
            var rect = new System.Windows.Int32Rect(x, y, frameWidth, frameHeight);
            var cropped = new CroppedBitmap(spriteBitmap, rect);
            imageControl.Source = cropped;
            if (!string.IsNullOrEmpty(sound))
            {
                var dataUrl = soundLoader.GetSoundDataUrl(sound);
                if (!string.IsNullOrEmpty(dataUrl)) soundPlayer?.PlayDataUrl(dataUrl);
            }
            currentFrame++;
            if (currentFrame >= frames.Count)
            {
                gestureAnimIndex++;
                PlayNextAnimation();
            }
            else
            {
                timer.Interval = TimeSpan.FromMilliseconds(frames[currentFrame].duration);
            }
        };

        PlayNextAnimation();
        timer.Start();
    }
}
