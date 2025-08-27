<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

## Project Overview
This project is a C# WPF desktop application for a digital pet. Focus on lightweight, fast startup, and extensibility for future features like animation, sound, and tricks.

## Code Style Guidelines
- Use PascalCase for class names, method names, and properties.
- Use camelCase for local variables and parameters.
- Prefer `var` when the type is obvious.
- Use async/await for asynchronous operations.
- Keep methods short and focused on a single responsibility.


## Folder Structure
- `src/DigitalPetApp/Views`: Contains all XAML UI files.
- `src/DigitalPetApp/ViewModels`: Contains view model classes.
- `src/DigitalPetApp/Models`: Contains data models.
- `src/DigitalPetApp/Services`: Contains business logic and data access services.
- `src/DigitalPetApp/Helpers`: Contains utility classes and converters.
- `src/DigitalPetApp/Features`: Contains feature implementations like animations, sounds, and tricks.
- `src/DigitalPetApp/Assets`: Contains animation and sound assets.
- `tests/DigitalPetApp.Tests`: Contains unit tests.

## Naming Conventions
- Views: `CustomerView.xaml`
- ViewModels: `CustomerViewModel.cs`
- Models: `Customer.cs`
- Services: `ICustomerService.cs`, `CustomerService.cs`
- Helpers: `DateTimeHelper.cs`, `BooleanToVisibilityConverter.cs`

## GitHub & Collaboration Notes
- The project is now ready for GitHub. Please ensure all new features (especially extensibility, animation, sound, and tricks) are implemented in a modular way.
- Use clear and short commit messages and descriptive pull requests when contributing.
- Keep the codebase lightweight and maintainable.

## Copilot Usage Tips
- Use Copilot to generate boilerplate code for ViewModels and Models.
- Ask Copilot to help with XAML bindings and layout suggestions.
- Use Copilot to refactor code for readability and performance.
- Use Copilot to write unit tests for services and ViewModels.

## Known Issues
- Navigation between views is currently manual; consider implementing a navigation service if more windows/pages are added.
- Some bindings may break if property names are changed without updating the XAML.
- Reminder feature currently counts seconds rather than true minutes (interval naming mismatch).

## Implemented
- Centralized file logging service (`FileLoggingService`).
- Feature modularity via `FeatureHost` and `ITogglableFeature`.
- Sound pooling (`PooledSoundPlayerService`).

## Future Enhancements
- Fix reminder interval (switch to minute-based or rename setting & UI label).
- Add localization support.
- Improve accessibility features (keyboard navigation, high contrast assets, screen reader text for notifications).
- Add unit tests for timer-driven features (ActivityMonitor, Reminder, RandomTrick).
- Provide a plugin discovery pattern or dynamic feature loading.
- Remove obsolete empty helpers (`AnimationHelper`, legacy `FeatureManager`) after documentation alignment.
