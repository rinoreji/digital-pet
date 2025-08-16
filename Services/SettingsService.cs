using System.Text.Json;
using System.IO;
using DigitalPetApp.Models;

namespace DigitalPetApp.Services;

public interface ISettingsService
{
    AppSettings Current { get; }
    void Save();
}

public class SettingsService : ISettingsService
{
    private readonly string settingsPath;
    public AppSettings Current { get; private set; } = new();
    private string? _lastSerialized; // for dirty tracking

    public SettingsService(string? customPath = null)
    {
        var baseDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;
        settingsPath = customPath ?? Path.Combine(baseDir, "appsettings.user.json");
        Load();
    }

    private void Load()
    {
        try
        {
            if (File.Exists(settingsPath))
            {
                var json = File.ReadAllText(settingsPath);
                var loaded = JsonSerializer.Deserialize<AppSettings>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (loaded != null) Current = loaded;
            }
        }
        catch { }
    }

    public void Save()
    {
        try
        {
        var json = JsonSerializer.Serialize(Current, new JsonSerializerOptions { WriteIndented = true });
        if (_lastSerialized == json) return; // no changes
        File.WriteAllText(settingsPath, json);
        _lastSerialized = json;
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[SettingsService] Saved settings to {settingsPath}: {json}");
#endif
        }
        catch { }
    }

    // Explicit conditional save helper
    public void SaveIfDirty() => Save();
}
