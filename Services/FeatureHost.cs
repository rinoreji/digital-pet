using DigitalPetApp.Features;

namespace DigitalPetApp.Services;

public class FeatureHost : IFeatureHost
{
    private readonly Dictionary<string, IAgentFeature> features = new();

    public void RegisterFeature(string key, IAgentFeature feature, bool start = true)
    {
        feature.Initialize();
        if (start) feature.Start();
        features[key] = feature;
    }

    public bool TryGet(string key, out IAgentFeature feature) => features.TryGetValue(key, out feature!);

    public void Start(string key)
    {
        if (features.TryGetValue(key, out var f)) f.Start();
    }

    public void Stop(string key)
    {
        if (features.TryGetValue(key, out var f)) f.Stop();
    }

    public void StartAll()
    {
        foreach (var f in features.Values) f.Start();
    }

    public void StopAll()
    {
        foreach (var f in features.Values) f.Stop();
    }

    public void UpdateAll()
    {
        foreach (var f in features.Values) f.Update();
    }

    public IEnumerable<Features.ITogglableFeature> GetTogglableFeatures()
        => features.Values.OfType<Features.ITogglableFeature>();

    public void SetEnabled(string key, bool enabled)
    {
        if (features.TryGetValue(key, out var f) && f is Features.ITogglableFeature tf)
        {
            if (tf.IsEnabled == enabled) return;
            if (enabled)
            {
                tf.IsEnabled = true;
                tf.Start();
            }
            else
            {
                tf.Stop();
                tf.IsEnabled = false;
            }
        }
    }
}
