namespace DigitalPetApp.Services;

// Obsolete: feature toggling moved into ITogglableFeature + FeatureHost
[System.Obsolete("Replaced by ITogglableFeature + FeatureHost.SetEnabled")] public interface IFeatureToggleService { }
[System.Obsolete("Replaced by ITogglableFeature + FeatureHost.SetEnabled")] public sealed class FeatureToggleService : IFeatureToggleService { }
