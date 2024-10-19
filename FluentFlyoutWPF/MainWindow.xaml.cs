﻿using System.Windows;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static WindowsMediaController.MediaManager;
using WindowsMediaController;
using Windows.Media.Control;
using System.Windows.Media.Imaging;
using Windows.Storage.Streams;
using System.Drawing;
using MicaWPF.Controls;
using Forms = System.Windows.Forms;
using System.IO;
using System.Windows.Media.Animation;


namespace FluentFlyoutWPF
{
    public partial class MainWindow : MicaWindow
    {
        //private HotKeyManager hotKeyManager;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_APPCOMMAND = 0x0319;

        private IntPtr _hookId = IntPtr.Zero;
        private LowLevelKeyboardProc _hookProc;

        private CancellationTokenSource cts;

        private static readonly MediaManager mediaManager = new MediaManager();
        private static MediaSession? currentSession = null;

        Forms.NotifyIcon notifyIcon = new NotifyIcon();

        public MainWindow()
        {
            InitializeComponent();
            Visibility = Visibility.Hidden;
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "icon.ico");
            notifyIcon.Icon = new Icon(path);
            notifyIcon.Text = "Media Flyout";
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Repository", null, openRepository);
            notifyIcon.ContextMenuStrip.Items.Add("Report bug", null, reportBug);
            notifyIcon.ContextMenuStrip.Items.Add("Exit", null, (s, e) => System.Windows.Application.Current.Shutdown());
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(5000, "Moved to tray", "The media flyout is running in the background", ToolTipIcon.Info);

            cts = new CancellationTokenSource();

            mediaManager.Start();

            _hookProc = HookCallback;
            _hookId = SetHook(_hookProc);

            WindowStartupLocation = WindowStartupLocation.Manual;
            Left = SystemParameters.WorkArea.Width/2 - Width/2;
            Top = SystemParameters.WorkArea.Height - Height - 60;

            mediaManager.OnAnyMediaPropertyChanged += MediaManager_OnAnyMediaPropertyChanged;
        }

        private void OpenAnimation()
        {
            var eventTriggers = this.Triggers[0] as EventTrigger;
            var beginStoryboard = eventTriggers.Actions[0] as BeginStoryboard;
            var storyboard = beginStoryboard.Storyboard;

            DoubleAnimation moveAnimation = (DoubleAnimation)storyboard.Children[0];
            moveAnimation.From = SystemParameters.WorkArea.Height - Height - 60;
            moveAnimation.To = SystemParameters.WorkArea.Height - Height - 80;

            DoubleAnimation opacityAnimation = (DoubleAnimation)storyboard.Children[1];
            opacityAnimation.From = 0;
            opacityAnimation.To = 1;

            storyboard.Begin(this);
        }

        private void reportBug(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void openRepository(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void MediaManager_OnAnyMediaPropertyChanged(MediaSession mediaSession, GlobalSystemMediaTransportControlsSessionMediaProperties mediaProperties)
        {
            UpdateUI(mediaManager.GetFocusedSession());
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_KEYUP))
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if (vkCode == 0xB3 || vkCode == 0xB0 || vkCode == 0xB1 || vkCode == 0xB2 // Play/Pause, next, previous, stop
                    || vkCode == 0xAD || vkCode == 0xAE || vkCode == 0xAF) // Mute, Volume Down, Volume Up
                {
                    ShowMediaFlyout();
                }
                else if (vkCode == 0xB0) // Next Track
                {
                    ShowMediaFlyout();
                }
                else if (vkCode == 0xB1) // Previous Track
                {
                    ShowMediaFlyout();
                }
                else if (vkCode == 0xB2) // Stop
                {
                    ShowMediaFlyout();
                }

            }
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private async void ShowMediaFlyout()
        {
            UpdateUI(mediaManager.GetFocusedSession());
            cts.Cancel();
            cts = new CancellationTokenSource();
            var token = cts.Token;

            if (Visibility == Visibility.Hidden) OpenAnimation();
            this.Visibility = Visibility.Visible;
            this.Topmost = true;

            try
            {
                await Task.Delay(3000, token);
                this.Hide();
            }
            catch (TaskCanceledException)
            {
                // do nothing
            }
        }

        private void UpdateUI(MediaSession mediaSession)
        {
            Dispatcher.Invoke(() =>
            {
            //var mediaProperties = mediaSession.ControlSession.GetPlaybackInfo();
            //if (mediaProperties != null)
            //{
            //    if (mediaSession.ControlSession.GetPlaybackInfo().Controls.IsPauseEnabled)
            //        ControlPlayPause.Content = "II";
            //    else
            //        ControlPlayPause.Content = "▶️";
            //    ControlBack.IsEnabled = ControlForward.IsEnabled = mediaProperties.Controls.IsNextEnabled;
            //}

            var songInfo = mediaSession.ControlSession.TryGetMediaPropertiesAsync().GetAwaiter().GetResult();
            if (songInfo != null)
            {
                SongTitle.Text = songInfo.Title;
                SongArtist.Text = songInfo.Artist;
                SongImage.Source = Helper.GetThumbnail(songInfo.Thumbnail);
            }
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            UnhookWindowsHookEx(_hookId);
            base.OnClosed(e);
            notifyIcon.Dispose();
        }

        internal static class Helper
        {
            internal static BitmapImage? GetThumbnail(IRandomAccessStreamReference Thumbnail, bool convertToPng = true)
            {
                if (Thumbnail == null)
                    return null;

                var thumbnailStream = Thumbnail.OpenReadAsync().GetAwaiter().GetResult();
                byte[] thumbnailBytes = new byte[thumbnailStream.Size];
                using (DataReader reader = new DataReader(thumbnailStream))
                {
                    reader.LoadAsync((uint)thumbnailStream.Size).GetAwaiter().GetResult();
                    reader.ReadBytes(thumbnailBytes);
                }

                byte[] imageBytes = thumbnailBytes;

                if (convertToPng)
                {
                    using var fileMemoryStream = new System.IO.MemoryStream(thumbnailBytes);
                    Bitmap thumbnailBitmap = (Bitmap)Bitmap.FromStream(fileMemoryStream);

                    if (!thumbnailBitmap.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
                    {
                        using var pngMemoryStream = new System.IO.MemoryStream();
                        thumbnailBitmap.Save(pngMemoryStream, System.Drawing.Imaging.ImageFormat.Png);
                        imageBytes = pngMemoryStream.ToArray();
                    }
                }

                var image = new BitmapImage();
                using (var ms = new System.IO.MemoryStream(imageBytes))
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                }

                return image;
            }
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);


        private void MicaWindow_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ShowMediaFlyout();
        }
    }
}