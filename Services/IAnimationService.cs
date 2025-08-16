using DigitalPetApp;

namespace DigitalPetApp.Services;

public interface IAnimationService
{
    // Queue a sequence of gestures; service ensures animations run sequentially.
    void PlaySequence(IEnumerable<Gestures> gestures);
    // High priority: inserted at the front of the queue; optionally can preempt current.
    void PlayPriority(IEnumerable<Gestures> gestures, bool interruptCurrent = false);
}
