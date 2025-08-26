namespace DigitalPetApp.Features;

public interface ITogglableFeature : IAgentFeature
{
    string Key { get; }
    string DisplayName { get; }
    bool IsEnabled { get; set; }
}
