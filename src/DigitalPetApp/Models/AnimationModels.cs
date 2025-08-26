namespace DigitalPetApp.Models;

public class AnimationRoot
{
    public int OverlayCount { get; set; }
    public List<string> Sounds { get; set; } = new();
    public List<int> Framesize { get; set; } = new();
    public Dictionary<string, Animation> Animations { get; set; } = new();
}

public class Animation
{
    public List<Frame> Frames { get; set; } = new();
}

public class Frame
{
    public int Duration { get; set; }
    public List<List<int>> Images { get; set; } = new();
    public string? Sound { get; set; }
}
