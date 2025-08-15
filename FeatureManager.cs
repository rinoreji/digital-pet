using System.Collections.Generic;

namespace DigitalPetApp
{
    public class FeatureManager
    {
        private readonly List<IAgentFeature> _features = new();

        public void RegisterFeature(IAgentFeature feature)
        {
            feature.Initialize();
            _features.Add(feature);
        }

        public void UpdateAll()
        {
            foreach (var feature in _features)
                feature.Update();
        }
    }
}
