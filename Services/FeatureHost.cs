using DigitalPetApp.Features;

namespace DigitalPetApp.Services;

public class FeatureHost : IFeatureHost
{
    private readonly List<IAgentFeature> features = new();

    public void RegisterFeature(IAgentFeature feature)
    {
        feature.Initialize();
        feature.Start();
        features.Add(feature);
    }

    public void StartAll()
    {
        foreach (var f in features) f.Start();
    }

    public void StopAll()
    {
        foreach (var f in features) f.Stop();
    }

    public void UpdateAll()
    {
        foreach (var f in features) f.Update();
    }
}
