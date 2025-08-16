using DigitalPetApp.Services;
using DigitalPetApp.Helpers;

namespace DigitalPetApp.Services;

public class SoundService : ISoundService
{
    public void PlayDataUrl(string? dataUrl) => SoundPlayerHelper.PlayDataUrl(dataUrl);
}
