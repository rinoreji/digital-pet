using System.Collections.Generic;

namespace DigitalPetApp.Features
{
    public class FeatureManager
    {
        private readonly List<IAgentFeature> _features = new();


        public void RegisterFeature(IAgentFeature feature)
        {
            feature.Initialize();
            feature.Start();
            _features.Add(feature);
        }

        public void StartAll()
        {
            foreach (var feature in _features)
                feature.Start();
        }

        public void StopAll()
        {
            foreach (var feature in _features)
                feature.Stop();
        }

        public void UpdateAll()
        {
            foreach (var feature in _features)
                feature.Update();
        }
    }
}
