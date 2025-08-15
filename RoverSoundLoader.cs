using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DigitalPetApp
{
    public class RoverSoundLoader
    {
        private readonly Dictionary<string, string> _sounds;

        public RoverSoundLoader(string jsonFilePath)
        {
            string json = File.ReadAllText(jsonFilePath);
            _sounds = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
        }

        public string? GetSoundDataUrl(string soundKey)
        {
            if (soundKey == null) return null;
            _sounds.TryGetValue(soundKey, out var dataUrl);
            return dataUrl;
        }

        public IEnumerable<string> Keys => _sounds.Keys;
    }
}
