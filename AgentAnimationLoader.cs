using System.IO;
using System.Text.Json;

namespace DigitalPetApp
{
    public class AgentAnimationLoader
    {

        public class AnimationRoot
        {
            public int OverlayCount { get; set; }
            public List<string> Sounds { get; set; }
            public List<int> Framesize { get; set; }
            public Dictionary<string, Animation> Animations { get; set; }
        }

        public class Animation
        {
            public List<Frame> Frames { get; set; }
        }

        public class Frame
        {
            public int Duration { get; set; }
            public List<List<int>> Images { get; set; }
            public string Sound { get; set; } // Optional, may be null
        }


        public Dictionary<string, Animation> Animations { get; } = new();

        public AgentAnimationLoader(string jsonFilePath)
        {
            string json = File.ReadAllText(jsonFilePath);
            var root = JsonSerializer.Deserialize<AnimationRoot>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
            if (root?.Animations != null)
            {
                foreach (var kvp in root.Animations)
                {
                    Animations[kvp.Key] = kvp.Value;
                }
            }
        }

        public List<(int x, int y, int duration, string? sound)> GetFrames(string animationName)
        {
            var result = new List<(int x, int y, int duration, string?)>();
            if (!Animations.ContainsKey(animationName) || Animations[animationName].Frames == null)
                return result;
            foreach (var f in Animations[animationName].Frames!)
            {
                if (f.Images != null && f.Images.Count > 0)
                {
                    var img = f.Images[0];
                    if (img.Count == 2)
                        result.Add((img[0], img[1], f.Duration, f.Sound));
                }
            }
            return result;
        }

        public Animation? GetAnimation(string name)
        {
            Animations.TryGetValue(name, out var anim);
            return anim;
        }
    }
}
