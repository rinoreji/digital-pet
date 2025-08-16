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
- `/Views`: Contains all XAML UI files.
- `/ViewModels`: Contains view model classes.
- `/Models`: Contains data models.
- `/Services`: Contains business logic and data access services.
- `/Helpers`: Contains utility classes and converters.
- `/Features`: Contains feature implementations like animations, sounds, and tricks.

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
- Navigation between views is currently manual; consider implementing a navigation service.
- Some bindings may break if property names are changed without updating the XAML.

## Future Enhancements
- Implement a centralized logging service.
- Add localization support.
- Improve accessibility features.
