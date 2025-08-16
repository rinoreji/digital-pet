namespace DigitalPetApp.Services;

public interface INavigationService
{
    void Navigate(string route, object? parameter = null);
}

public class NavigationService : INavigationService
{
    public void Navigate(string route, object? parameter = null)
    {
        // Placeholder for future multi-view navigation
    }
}
