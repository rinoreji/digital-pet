namespace DigitalPetApp.Services;

[System.Obsolete("Use ISoundPlayerService / PooledSoundPlayerService instead")] public interface ISoundService { void PlayDataUrl(string? dataUrl); }
