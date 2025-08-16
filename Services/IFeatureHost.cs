namespace DigitalPetApp.Services;

public interface IFeatureHost
{
    void RegisterFeature(Features.IAgentFeature feature);
    void StartAll();
    void StopAll();
    void UpdateAll();
}
