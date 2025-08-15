using System;
using System.Windows;
using System.Windows.Threading;

namespace DigitalPetApp
{
    public partial class BalloonNotificationWindow : Window
    {
        private readonly DispatcherTimer _closeTimer;

        public BalloonNotificationWindow(string message, string title = "Notification", int durationMs = 3500)
        {
            InitializeComponent();
            TitleText.Text = title;
            MessageText.Text = message;
            _closeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(durationMs) };
            _closeTimer.Tick += (s, e) => { _closeTimer.Stop(); this.Close(); };
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            _closeTimer.Start();
        }
    }
}
