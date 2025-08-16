using System.Windows;

namespace DigitalPetApp.Services;

public class BalloonPositionService : IBalloonPositionService
{
    public (double left, double top) GetBalloonPosition(Window balloon, FrameworkElement? anchorElement)
    {
        if (anchorElement != null)
        {
            var point = anchorElement.PointToScreen(new Point(0, 0));
            var source = PresentationSource.FromVisual(anchorElement);
            double dpiX = 1.0, dpiY = 1.0;
            if (source != null)
            {
                dpiX = source.CompositionTarget.TransformToDevice.M11;
                dpiY = source.CompositionTarget.TransformToDevice.M22;
            }
            double width = balloon.ActualWidth > 0 ? balloon.ActualWidth : balloon.Width;
            double height = balloon.ActualHeight > 0 ? balloon.ActualHeight : balloon.Height;
            double dogImageCenterX = point.X / dpiX + anchorElement.ActualWidth / 2;
            double balloonLeft = dogImageCenterX - width / 2 - 50;
            double balloonTop = point.Y / dpiY - height;
            return (balloonLeft, balloonTop);
        }
        var desktop = SystemParameters.WorkArea;
        double fallbackWidth = balloon.ActualWidth > 0 ? balloon.ActualWidth : balloon.Width;
        double fallbackHeight = balloon.ActualHeight > 0 ? balloon.ActualHeight : balloon.Height;
        return (desktop.Right - fallbackWidth - 10, desktop.Bottom - fallbackHeight - 10);
    }
}
