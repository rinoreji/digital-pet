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
- Animated digital pet (dog/rover)
- Clickable pet with fun animations
- Notification and reminder support
- Extensible architecture for adding new features (animations, sounds, tricks)


## 🖼️ Screenshots
<!-- Add screenshots here if available -->

## 🚀 Getting Started
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
	  dotnet build
	  ```
4. **Run the application:**
	- Using Visual Studio: Press `F5` or click `Debug > Start Debugging`.
	- Using VS Code or terminal:
	  ```
	  dotnet run
	  ```

## 🕹️ Usage
- The digital pet appears in a small window on your desktop.
- Click the pet to trigger a fun animation.
- Notifications and reminders can be triggered by features (see `FeatureManager`).

## 🧩 Extending the App
- Add new animations: See `AnimationHelper` and `AgentAnimationLoader`.
- Add new sounds: See `SoundPlayerHelper` and `RoverSoundLoader`.
- Add new features: Implement `IAgentFeature` and register with `FeatureManager`.

## 🗂️ Project Structure
- `MainWindow.xaml` / `.cs`: Main UI and logic
- `Assets/`: Images, animation data, and sound assets
- `FeatureManager.cs`: Feature registration and management
- `AnimationHelper.cs`, `SoundPlayerHelper.cs`: Animation and sound utilities

## 🤝 Contributing
Pull requests are welcome! For major changes, please open an issue first to discuss what you would like to change.

## 📄 License
MIT License. See [LICENSE](LICENSE) for details.

## 🙏 Credits
- Inspired by classic desktop pets and assistants, especially Microsoft Agent Rover
- Agent animation and sound files adapted from [clippy.js](https://github.com/clippyjs/clippy.js) — thank you to the clippy.js project!
- Built with .NET 9.0 and WPF

---
*This project is ready for your ideas—animations, sounds, tricks, and more!*
