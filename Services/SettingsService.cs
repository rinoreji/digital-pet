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
            File.WriteAllText(settingsPath, json);
        }
        catch { }
    }
}
