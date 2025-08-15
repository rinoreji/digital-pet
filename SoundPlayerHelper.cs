using System;
using System.IO;
using System.Media;
using System.Windows.Media;

namespace DigitalPetApp
{
    public static class SoundPlayerHelper
    {
        // Plays a base64 data URL (data:audio/mpeg;base64,...) using a temporary file and MediaPlayer
        public static void PlayDataUrl(string? dataUrl)
        {
            if (string.IsNullOrEmpty(dataUrl) || !dataUrl.StartsWith("data:audio/mpeg;base64,"))
                return;
            try
            {
                var base64 = dataUrl.Substring("data:audio/mpeg;base64,".Length);
                byte[] bytes = Convert.FromBase64String(base64);
                string tempFile = Path.GetTempFileName() + ".mp3";
                File.WriteAllBytes(tempFile, bytes);
                var player = new MediaPlayer();
                player.Open(new Uri(tempFile));
                player.Play();
                // Clean up temp file after playback
                player.MediaEnded += (s, e) =>
                {
                    player.Close();
                    try { File.Delete(tempFile); } catch { }
                };
            }
            catch { /* Ignore errors */ }
        }
    }
}
