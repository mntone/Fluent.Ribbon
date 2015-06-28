namespace Fluent.Internal
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;
    using Fluent.Metro.Native;
    using System.Windows.Media;

    /// <summary>
    /// Encapsulates logic for window sizing (maximizing etc.)
    /// </summary>
    public class WindowSizing
    {
        private readonly RibbonWindow window;
        private HwndSource windowSource;
        private bool fixingNastyWindowChromeBug;

        private bool isPerMonitorDpiSupported = false;
        internal Dpi systemDpi;
        private Dpi currentDpi;

        /// <summary>
        /// Creates a new instance and binds it to <paramref name="window"/>
        /// </summary>
        public WindowSizing(RibbonWindow window)
        {
            this.window = window;
        }

        /// <summary>
        /// Called when <see cref="window"/> has been initialize
        /// </summary>
        public void WindowInitialized()
        {
            var hwndSource = PresentationSource.FromVisual(this.window) as HwndSource;
            if (hwndSource == null) return;

            this.windowSource = hwndSource;
            hwndSource.AddHook(this.HwndHook);

            this.systemDpi = this.GetSystemDpi();
            this.isPerMonitorDpiSupported = PerMonitorDpi.IsSupported;
            if (this.isPerMonitorDpiSupported)
            {
                this.currentDpi = this.windowSource.GetMonitorDpi();
                this.ChangeDpi(this.currentDpi);
            }
            else
            {
                this.currentDpi = this.systemDpi;
            }
        }

        private IntPtr HwndHook(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var returnval = IntPtr.Zero;

            switch (message)
            {
                case Constants.WM_DPICHANGED:
                    if (this.isPerMonitorDpiSupported)
                    {
                        var dpiX = wParam.ToLoWord();
                        var dpiY = wParam.ToHiWord();
                        this.ChangeDpi(new Dpi(dpiX, dpiY));
                        handled = true;
                    }
                    break;

                case Constants.WM_NCLBUTTONDOWN:
                    this.window.isSystemMenuOpened = false;
                    break;
            }

            return returnval;
        }

        #region Dpi

        private void ChangeDpi(Dpi dpi)
        {
            this.window.DpiScaleTransform = (dpi == this.systemDpi)
                   ? Transform.Identity
                   : new ScaleTransform((double)dpi.X / this.systemDpi.X, (double)dpi.Y / this.systemDpi.Y);

            this.window.Width = this.window.Width * dpi.X / this.currentDpi.X;
            this.window.Height = this.window.Height * dpi.Y / this.currentDpi.Y;
            this.currentDpi = dpi;
        }

        private Dpi GetSystemDpi()
        {
            return new Dpi(
                (ushort)(Dpi.Default.X * windowSource.CompositionTarget.TransformToDevice.M11),
                (ushort)(Dpi.Default.Y * windowSource.CompositionTarget.TransformToDevice.M22));
        }

        public Point PhysicalToVirtualBySystemDpi(Point point)
        {
            return new Point(point.X / this.systemDpi.ScaleX, point.Y / this.systemDpi.ScaleY);
        }

        #endregion

        #region GlassSize

        internal Tuple<double, double> GetBorderWidthAndCaptionHeight()
        {
            var metrics = new NonClientMetrics();
            metrics.cbSize = Marshal.SizeOf(metrics);
            NativeMethods.SystemParametersInfo(NativeMethods.SPI_GETNONCLIENTMETRICS, metrics.cbSize, ref metrics, 0);

            // [real border size] = 3 + scaling * borderWidth + scaling * paddedBorderWidth
            // * This function return a value scaled by scaling.
            var scaling = this.systemDpi.IsZero ? 1.0 : this.systemDpi.ScaleX;
            return Tuple.Create(
                (3.0 + (double)metrics.iBorderWidth + (double)metrics.iPaddedBorderWidth) / scaling,
                (double)metrics.iCaptionHeight / scaling);
        }

        #endregion
    }
}