using DigitalPetApp.Features;

namespace DigitalPetApp.Services;

public interface IFeatureToggleService
{
    bool IsEnabled(string featureKey);
    void SetEnabled(string featureKey, bool enabled);
    event Action<string,bool>? FeatureToggled;
}

public class FeatureToggleService : IFeatureToggleService
{
    private readonly Dictionary<string,bool> _states = new();
    public event Action<string,bool>? FeatureToggled;

    public bool IsEnabled(string featureKey)
        => _states.TryGetValue(featureKey, out var v) ? v : true;

    public void SetEnabled(string featureKey, bool enabled)
    {
        if (_states.TryGetValue(featureKey, out var existing) && existing == enabled) return;
        _states[featureKey] = enabled;
        FeatureToggled?.Invoke(featureKey, enabled);
    }
}
