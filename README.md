## 🔖 Tagline
**Reimagining Rover — a nostalgic nod to Microsoft Agent, powered by GitHub Copilot.**

---

## 📘 Project Introduction  
Welcome to a modern recreation of **Microsoft Agent Rover**, the iconic animated assistant from the early Windows era. This project is a fun and experimental tribute, built with the help of **GitHub Copilot**, combining retro charm with modern development tools.

---

## 🐶 Short Description  
A playful recreation of Microsoft Agent Rover — built with the help of GitHub Copilot to bring nostalgic charm to life.

---


## ✨ Features
- Lightweight and fast startup
- Animated digital pet (Rover)
- Clickable pet with fun animations & sounds
- Notifications (balloon) + reminder & hourly chime
- Idle / random trick behaviors (activity aware)
- Modular feature host (`FeatureHost`) for easy plug‑ins
- Persisted user settings (feature toggles, idle timeout, audio, window position) in `appsettings.user.json`

---

## 🗂️ Solution Structure

- `src/DigitalPetApp/` — Main application code
	- `Features/` — Feature modules (animations, reminders, etc.)
	- `Helpers/` — Utility classes and converters
	- `Models/` — Data models
	- `Services/` — Business logic and services
	- `ViewModels/` — MVVM view models
	- `Views/` — XAML UI files
	- `Assets/` — Animation and sound assets (e.g., `Assets/rover/agent.json`)
- `tests/DigitalPetApp.Tests/` — Unit tests

---


## 🖼️ Screenshots
<!-- Add screenshots here if available -->


## �️ Getting Started
1. **Clone the repository:**
	```
	git clone https://github.com/rinoreji/digital-pet.git
	cd digital-pet
	```
2. **Open in Visual Studio or VS Code.**
3. **Build the project:**
	- Using Visual Studio: Press `F5` or click `Build > Build Solution`.
	- Using VS Code: Run the build task or use the terminal:
	  ```
	  dotnet build src/DigitalPetApp/DigitalPetApp.csproj
	  ```
4. **Run the application:**
	- Using Visual Studio: Press `F5` or click `Debug > Start Debugging`.
	- Using VS Code or terminal:
	  ```
	  dotnet run --project src/DigitalPetApp/DigitalPetApp.csproj
	  ```

## 🕹️ Usage
- The pet window docks bottom‑right by default (position remembered across runs).
- Drag the window to reposition; click the pet to trigger an animation + greeting.
- Feature behaviors (reminder, hourly chime, idle animation, random trick) run automatically if enabled.
- Open Settings (tray icon or context action) to tweak timers, feature toggles, and audio.

## 🧩 Extending the App
### Animations
Frames & timing are defined in `src/DigitalPetApp/Assets/rover/agent.json`. Each gesture name maps to a sequence of frames (`x`,`y`,`duration`,`sound`). At runtime `AgentAnimationLoader` + `SpriteSheetAnimationService` handle playback. To add a new gesture:
1. Add frames & optional `sound` keys to `agent.json` under a new animation name.
2. Reference that gesture via a new enum value in `Gestures` (if not already present).
3. Call `IAnimationService.PlaySequence(new[]{ Gestures.NewGesture })` from a feature/view model.

### Sounds
Base64 MP3 data URLs are stored in `Assets/rover/sounds-mp3.json` and looked up by key via `RoverSoundLoader`, then cached & played through `PooledSoundPlayerService`.

### Features
Implement `ITogglableFeature` (inherits `IAgentFeature`) and register it in `MainWindow` (or a future DI bootstrap) via `FeatureHost.RegisterFeature("Key", feature, start: feature.IsEnabled)`. Use `AgentTimerService` ticks or `ActivityMonitor` events for timing/idle logic.

### Persisted Settings
`SettingsService` serializes `AppSettings` to `appsettings.user.json` (created on first run). Add new user preferences by extending `AppSettings` and wiring save/load + binding.

### Example: skeleton feature
```csharp
public class WaveFeature : ITogglableFeature {
	public string Key => "Wave"; public string DisplayName => "Wave"; public bool IsEnabled { get; set; } = true;
	private readonly IAnimationService anim; private readonly AgentTimerService timer;
	public WaveFeature(IAnimationService anim, AgentTimerService timer){ this.anim=anim; this.timer=timer; }
	public void Initialize(){}
	public void Start(){ if (!IsEnabled) return; timer.MinuteTick += OnMinute; }
	public void Stop(){ timer.MinuteTick -= OnMinute; }
	public void Update(){}
	private void OnMinute(){ anim.PlaySequence(new[]{ Gestures.Greet }); }
}
```

## 🗂️ Project Structure
High-level layout:
- `Views/` – WPF XAML (`MainWindow`, `BalloonNotificationWindow`, `SettingsWindow`)
- `ViewModels/` – MVVM layers (`MainViewModel`, `SettingsViewModel`)
- `Models/` – Data (`AnimationModels`, `AppSettings`)
- `Services/` – Core services (animation, sound pool, notifications, timers, feature host, logging, settings, tray)
- `Features/` – Modular behaviors (`ReminderFeature`, `HourlyChimeFeature`, `IdleAnimationFeature`, `RandomTrickFeature`)
- `Helpers/` – Loaders (`AgentAnimationLoader`, `RoverSoundLoader`) + small legacy shims
- `Assets/` – Sprite map, animation & sound JSON, icons
- `Gestures.cs` – Enum of available animation gesture names

`FeatureHost` supersedes the older `FeatureManager` concept (legacy file removed).

## 🔧 Current Features (Built‑in)
| Key | Display Name | Purpose | Trigger Source |
|-----|--------------|---------|----------------|
| Reminder | Reminder | Periodic break notification + attention animation | Second tick counter (intended minutes) |
| HourlyChime | Hourly Chime | Top-of-hour notification & greet animation | HourTick |
| IdleAnimation | Idle Animation | Play 1–2 random low-key animations on user idle | ActivityMonitor IdleStarted |
| RandomTrick | Random Trick | Occasional trick animation; accelerates when idle | SecondTick + idle heuristics |

## 💾 Persisted Settings (`AppSettings`)
```jsonc
{
	"IdleTimeoutSeconds": 60,
	"ReminderIntervalMinutes": 10,
	"EnableReminder": true,
	"EnableHourlyChime": true,
	"EnableIdleAnimation": true,
	"EnableRandomTrick": true,
	"Volume": 0.8,
	"Muted": false,
	"WindowLeft": 123.0, // (optional)
	"WindowTop": 456.0   // (optional)
}
```

## ⚠️ Known Issues
- Reminder interval currently counts seconds, not minutes (logic uses `SecondTick`). Intention is minutes; pending fix.
- Duplicate gesture entries in some random pools act as implicit weighting; clarify or deduplicate later.
- (Resolved) Legacy empty files removed (`FeatureManager.cs`, `AnimationHelper.cs`, `SoundPlayerHelper.cs`).

## 🗺️ Roadmap Ideas
- Correct reminder timing to true minutes (or rename setting to seconds).
- Mood/state system influencing random animations.
- Pluggable gesture pack loading.
- UI for feature enable/disable without opening Settings (tray quick menu).
- Basic unit tests for timing & activity logic.

## 🤝 Contributing
Pull requests are welcome! For major changes, please open an issue first to discuss what you would like to change.

## 📄 License
MIT License. See [LICENSE](LICENSE) for details.

## 🙏 Credits
- Inspired by classic desktop pets / Microsoft Agent Rover.
- Animation & sound data adapted from [clippy.js](https://github.com/clippyjs/clippy.js).
- Built with .NET 9.0 (TargetFramework `net9.0-windows`) and WPF.

---
*This project is ready for your ideas—animations, sounds, tricks, and more!*
