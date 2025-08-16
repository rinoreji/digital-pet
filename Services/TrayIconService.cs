using System;
using System.Windows;

namespace DigitalPetApp.Services;

public interface ITrayIconService : IDisposable
{
    void Show();
}

public class TrayIconService : ITrayIconService
{
    private readonly System.Windows.Forms.NotifyIcon notifyIcon;
    private readonly Window mainWindow;
    private readonly Action openSettings;

    public TrayIconService(Window mainWindow, Action openSettings)
    {
        this.mainWindow = mainWindow;
        this.openSettings = openSettings;
    notifyIcon = new System.Windows.Forms.NotifyIcon();
        notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName!);
        notifyIcon.Text = "Digital Pet";
        notifyIcon.Visible = true;
        notifyIcon.ContextMenuStrip = BuildMenu();
        notifyIcon.DoubleClick += (_, _) => ShowMainWindow();
    }

    private System.Windows.Forms.ContextMenuStrip BuildMenu()
    {
        var menu = new System.Windows.Forms.ContextMenuStrip();
        menu.Items.Add("Show", null, (_, _) => ShowMainWindow());
        menu.Items.Add("Settings", null, (_, _) => openSettings());
        menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
    menu.Items.Add("Exit", null, (_, _) => System.Windows.Application.Current.Shutdown());
        return menu;
    }

    private void ShowMainWindow()
    {
        if (mainWindow.WindowState == WindowState.Minimized)
            mainWindow.WindowState = WindowState.Normal;
        mainWindow.Show();
        mainWindow.Activate();
    }

    public void Show() { notifyIcon.Visible = true; }

    public void Dispose()
    {
        notifyIcon.Visible = false;
        notifyIcon.Dispose();
    }
}
