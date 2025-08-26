using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace DigitalPetApp.Helpers;

public static class ClickCommandBehavior
{
    public static readonly DependencyProperty ClickCommandProperty = DependencyProperty.RegisterAttached(
        "ClickCommand",
        typeof(ICommand),
        typeof(ClickCommandBehavior),
        new PropertyMetadata(null, OnClickCommandChanged));

    public static void SetClickCommand(DependencyObject obj, ICommand value) => obj.SetValue(ClickCommandProperty, value);
    public static ICommand? GetClickCommand(DependencyObject obj) => (ICommand?)obj.GetValue(ClickCommandProperty);

    private static void OnClickCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement element)
        {
            element.MouseLeftButtonUp -= Element_MouseLeftButtonUp;
            if (e.NewValue is ICommand)
            {
                element.MouseLeftButtonUp += Element_MouseLeftButtonUp;
            }
        }
    }

    private static void Element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is DependencyObject d)
        {
            var cmd = GetClickCommand(d);
            if (cmd?.CanExecute(null) == true)
            {
                cmd.Execute(null);
            }
        }
    }
}
