namespace DigitalPetApp.Services;

[System.Obsolete("Use PooledSoundPlayerService instead")] public class SoundService : ISoundService
{
    public void PlayDataUrl(string? dataUrl) { /* legacy no-op */ }
}
