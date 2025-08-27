using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Collections.Generic;

namespace DigitalPetApp.Services
{
    /// <summary>
    /// Registers and listens for global (system-wide) hotkeys.
    /// Simplified service supporting one-off registrations with callback.
    /// </summary>
    public sealed class GlobalHotkeyService : IDisposable
    {
        private readonly HwndSource hwndSource;
        private int currentId = 0x0000;
        private bool disposed;

        private const int WM_HOTKEY = 0x0312;

        private class Registration
        {
            public int Id { get; init; }
            public Action Callback { get; init; } = () => { };
        }

        private readonly Dictionary<int, Registration> registrations = new();

        public GlobalHotkeyService(System.Windows.Window window)
        {
            var source = (HwndSource)HwndSource.FromHwnd(new WindowInteropHelper(window).Handle);
            if (source == null)
            {
                throw new InvalidOperationException("Window handle source not available yet. Call after SourceInitialized.");
            }
            hwndSource = source;
            hwndSource.AddHook(WndProc);
        }

        [Flags]
        private enum Modifiers : uint
        {
            None = 0x0000,
            Alt = 0x0001,
            Control = 0x0002,
            Shift = 0x0004,
            Win = 0x0008
        }

        public int Register(ModifierKeys modifierKeys, Key key, Action callback)
        {
            if (disposed) throw new ObjectDisposedException(nameof(GlobalHotkeyService));
            currentId++;
            var id = currentId;
            var mods = ToModifiers(modifierKeys);
            var vk = (uint)KeyInterop.VirtualKeyFromKey(key);
            if (!RegisterHotKey(hwndSource.Handle, id, (uint)mods, vk))
            {
                throw new InvalidOperationException($"Failed to register hotkey {modifierKeys}+{key} (id {id}).");
            }
            registrations[id] = new Registration { Id = id, Callback = callback };
            return id;
        }

        public void Unregister(int id)
        {
            if (registrations.Remove(id))
            {
                UnregisterHotKey(hwndSource.Handle, id);
            }
        }

        private Modifiers ToModifiers(ModifierKeys keys)
        {
            Modifiers mods = Modifiers.None;
            if (keys.HasFlag(ModifierKeys.Control)) mods |= Modifiers.Control;
            if (keys.HasFlag(ModifierKeys.Alt)) mods |= Modifiers.Alt;
            if (keys.HasFlag(ModifierKeys.Shift)) mods |= Modifiers.Shift;
            if (keys.HasFlag(ModifierKeys.Windows)) mods |= Modifiers.Win;
            return mods;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                var id = wParam.ToInt32();
                if (registrations.TryGetValue(id, out var reg))
                {
                    reg.Callback();
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            foreach (var id in registrations.Keys)
            {
                UnregisterHotKey(hwndSource.Handle, id);
            }
            registrations.Clear();
            hwndSource.RemoveHook(WndProc);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}
