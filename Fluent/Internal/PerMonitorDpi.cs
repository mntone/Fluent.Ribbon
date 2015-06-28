namespace Fluent.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Interop;

    internal static class PerMonitorDpi
    {
        public static bool IsSupported
        {
            get
            {
                var version = Environment.OSVersion.Version;
                if (version.Major == 6 && version.Minor >= 3 || version.Major >= 7)
                {
                    var awareness = ProcessDpiAwareness.DpiUnaware;
                    NativeMethods.GetProcessDpiAwareness(IntPtr.Zero, out awareness);
                    return awareness == ProcessDpiAwareness.PerMonitorDpiAware;
                }
                return false;
            }
        }

        public static Dpi GetMonitorDpi(this HwndSource hwndSource,
            MonitorDefaultTo defaultTo = MonitorDefaultTo.Nearest,
            MonitorDpiType dpiType = MonitorDpiType.Default)
        {
            if (!IsSupported) return Dpi.Default;

            var hmonitor = NativeMethods.MonitorFromWindow(
                hwndSource.Handle,
                defaultTo);

            uint dpiX = 1, dpiY = 1;
            NativeMethods.GetDpiForMonitor(hmonitor, dpiType, ref dpiX, ref dpiY);

            return new Dpi((ushort)dpiX, (ushort)dpiY);
        }
    }
}