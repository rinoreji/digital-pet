using System.Collections.Concurrent;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media;

namespace DigitalPetApp.Services;

public interface ISoundPlayerService
{
    void PlayDataUrl(string? dataUrl);
    double Volume { get; set; }
    bool Muted { get; set; }
}

public class PooledSoundPlayerService : ISoundPlayerService, IDisposable
{
    private readonly string cacheDir;
    private readonly ConcurrentDictionary<string,string> cachedFiles = new();
    private readonly ConcurrentBag<MediaPlayer> pool = new();
    private readonly int poolSize;
    private double volume = 0.8; // default
    private bool muted = false;

    public PooledSoundPlayerService(int poolSize = 3)
    {
        this.poolSize = poolSize;
    cacheDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "DigitalPetSoundCache");
    System.IO.Directory.CreateDirectory(cacheDir);
    }

    public void PlayDataUrl(string? dataUrl)
    {
        if (string.IsNullOrEmpty(dataUrl) || !dataUrl.StartsWith("data:audio/mpeg;base64,")) return;
        try
        {
            var hash = Hash(dataUrl);
            var file = cachedFiles.GetOrAdd(hash, h =>
            {
                var base64 = dataUrl.Substring("data:audio/mpeg;base64,".Length);
                var bytes = Convert.FromBase64String(base64);
                var path = Path.Combine(cacheDir, h + ".mp3");
                File.WriteAllBytes(path, bytes);
                return path;
            });
            var player = GetPlayer();
            player.Open(new Uri(file));
            player.MediaEnded += OnEnded;
            if (!muted)
            {
                player.Volume = volume;
                player.Play();
            }

            void OnEnded(object? s, EventArgs e)
            {
                player.MediaEnded -= OnEnded;
                player.Stop();
                ReturnPlayer(player);
            }
        }
        catch { }
    }

    private MediaPlayer GetPlayer()
    {
        if (!pool.TryTake(out var p)) p = new MediaPlayer();
        return p;
    }

    public double Volume { get => volume; set { volume = Math.Clamp(value, 0, 1); } }
    public bool Muted { get => muted; set { muted = value; } }

    private void ReturnPlayer(MediaPlayer p)
    {
        if (pool.Count < poolSize) pool.Add(p); else { p.Close(); }
    }

    private static string Hash(string input)
    {
        using var sha = SHA1.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return string.Concat(bytes.Select(b => b.ToString("x2")));
    }

    public void Dispose()
    {
        while (pool.TryTake(out var p)) { try { p.Close(); } catch { } }
    }
}
