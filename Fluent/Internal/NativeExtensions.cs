﻿namespace Fluent.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;

    internal static class NativeExtensions
    {
        public static ushort ToLoWord(this IntPtr dword)
        {
            return (ushort)((uint)dword & 0xffff);
        }

        public static ushort ToHiWord(this IntPtr dword)
        {
            return (ushort)(((uint)dword >> 16) & 0xffff);
        }
    }
}