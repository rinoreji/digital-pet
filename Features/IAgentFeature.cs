namespace DigitalPetApp.Features
{
    public interface IAgentFeature
    {
        void Initialize();
        void Start();
        void Stop();
        void Update(); // Called periodically or on demand
    }
}
