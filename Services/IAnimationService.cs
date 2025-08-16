using DigitalPetApp;

namespace DigitalPetApp.Services;

public interface IAnimationService
{
    void PlaySequence(IEnumerable<Gestures> gestures);
}
