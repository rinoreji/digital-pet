using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DigitalPetApp
{
    public static class AnimationHelper
    {
        // Plays a sequence of animations in order, updating the given Image control
    private static AgentAnimationLoader? _animationLoader;
    private static RoverSoundLoader? _soundLoader;
    private static Image? _imageControl;
    private static string? _spriteSheetPath;
    private static int _frameWidth;
    private static int _frameHeight;

        public static void Init(
            AgentAnimationLoader animationLoader,
            RoverSoundLoader soundLoader,
            Image imageControl,
            string spriteSheetPath,
            int frameWidth,
            int frameHeight)
        {
            _animationLoader = animationLoader ?? throw new ArgumentNullException(nameof(animationLoader));
            _soundLoader = soundLoader ?? throw new ArgumentNullException(nameof(soundLoader));
            _imageControl = imageControl ?? throw new ArgumentNullException(nameof(imageControl));
            _spriteSheetPath = spriteSheetPath ?? throw new ArgumentNullException(nameof(spriteSheetPath));
            _frameWidth = frameWidth;
            _frameHeight = frameHeight;
        }

        public static void PlayAnimationSequence(List<Gestures> animationTypes)
        {
            if (_animationLoader == null) throw new InvalidOperationException("AnimationHelper not initialized. Call Init first.");
            if (_soundLoader == null) throw new InvalidOperationException("AnimationHelper not initialized. Call Init first.");
            if (_imageControl == null) throw new InvalidOperationException("AnimationHelper not initialized. Call Init first.");
            if (_spriteSheetPath == null) throw new InvalidOperationException("AnimationHelper not initialized. Call Init first.");
            if (animationTypes == null || animationTypes.Count == 0) return;

            var spriteUri = new Uri(_spriteSheetPath, UriKind.Absolute);
            var spriteBitmap = new BitmapImage(spriteUri);
            int animIndex = 0;
            List<(int x, int y, int duration, string? sound)>? frames = null;
            int currentFrame = 0;
            DispatcherTimer timer = new DispatcherTimer();

            void PlayNextAnimation()
            {
                if (animIndex >= animationTypes.Count)
                {
                    timer.Stop();
                    return;
                }
                var animType = animationTypes[animIndex];
                frames = _animationLoader!.GetFrames(animType.ToString());
                currentFrame = 0;
                if (frames == null || frames.Count == 0)
                {
                    animIndex++;
                    PlayNextAnimation();
                    return;
                }
                timer.Interval = TimeSpan.FromMilliseconds(frames[0].duration);
            }

            timer.Tick += (s, e) =>
            {
                if (frames == null || frames.Count == 0)
                {
                    animIndex++;
                    PlayNextAnimation();
                    return;
                }
                var (x, y, duration, sound) = frames[currentFrame];
                var rect = new System.Windows.Int32Rect(x, y, _frameWidth, _frameHeight);
                var cropped = new CroppedBitmap(spriteBitmap, rect);
                _imageControl.Source = cropped;
                // Play sound if specified
                if (!string.IsNullOrEmpty(sound))
                {
                    var dataUrl = _soundLoader!.GetSoundDataUrl(sound);
                    if (!string.IsNullOrEmpty(dataUrl))
                    {
                        SoundPlayerHelper.PlayDataUrl(dataUrl);
                    }
                }
                currentFrame++;
                if (currentFrame >= frames.Count)
                {
                    animIndex++;
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
}
