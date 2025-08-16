namespace DigitalPetApp.Services;

public interface IFeatureHost
{
    // New keyed registration
    void RegisterFeature(string key, Features.IAgentFeature feature, bool start = true);
    void StartAll();
    void StopAll();
    void UpdateAll();
    void Start(string key);
    void Stop(string key);
    bool TryGet(string key, out Features.IAgentFeature feature);
    System.Collections.Generic.IEnumerable<Features.ITogglableFeature> GetTogglableFeatures();
    void SetEnabled(string key, bool enabled);

    // Backward compatibility default helper (can be removed later)
    void RegisterFeature(Features.IAgentFeature feature) => RegisterFeature(feature.GetType().Name, feature, start: true);
}
