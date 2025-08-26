using System.Windows;

namespace DigitalPetApp.Services;

public interface IBalloonPositionService
{
    (double left, double top) GetBalloonPosition(Window balloon, FrameworkElement? anchorElement);
}
