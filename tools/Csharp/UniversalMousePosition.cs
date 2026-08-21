#if UNITY_EDITOR && UNITY_EDITOR_WIN

using System;
using System.Runtime.InteropServices;
using System.Drawing;
using UnityEngine;

namespace Tools
{
    public static class UniversalMousePosition
    {
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

        [DllImport("shcore.dll")]
        private static extern int GetDpiForMonitor(IntPtr hmonitor, int dpiType, out uint dpiX, out uint dpiY);
        private delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

        private const uint MONITOR_DEFAULT_TO_NEAREST = 2;

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MONITORINFO
        {
            public uint cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            private int Width => Right - Left;
            private int Height => Bottom - Top;

            public override string ToString()
            {
                return $"Left: {Left}, Top: {Top}, Right: {Right}, Bottom: {Bottom}";
            }
        }

        /// <summary>
        /// Scale size
        /// </summary>
        public static float GetCurrentMonitorScale()
        {
            POINT pt;
            GetCursorPos(out pt);
            IntPtr hMonitor = MonitorFromPoint(pt, MONITOR_DEFAULT_TO_NEAREST);

            // 0 = MDT_EFFECTIVE_DPI - dpiType
            if (GetDpiForMonitor(hMonitor, 0, out uint dpiX, out uint _) == 0)
            {
                return dpiX / 96f; //96 - win standard
            }

            return 1.0f;
        }

        /// <summary>
        /// Cursor pos aware screen scale
        /// </summary>
        public static Vector2 GetScaledCursorPosition()
        {
            POINT pt;
            GetCursorPos(out pt);
            float scale = GetCurrentMonitorScale();

            return new Vector2(pt.X / scale, pt.Y / scale);
        }

        public static Vector2Int GetRawCursorPosition()
        {
            POINT pt;
            GetCursorPos(out pt);
            return new Vector2Int(pt.X, pt.Y);
        }

        public static Vector2Int GetCursorPosition()
        {
            GetCursorPos(out POINT lpPoint);
            return new Vector2Int(lpPoint.X, lpPoint.Y);
        }

        public static RECT GetCurrentMonitorRect()
        {
            GetCursorPos(out POINT pt);
            IntPtr hMonitor = MonitorFromPoint(pt, MONITOR_DEFAULT_TO_NEAREST);

            MONITORINFO info = new MONITORINFO();
            info.cbSize = (uint)Marshal.SizeOf(typeof(MONITORINFO));

            return GetMonitorInfo(hMonitor, ref info) ? info.rcMonitor : new RECT();
        }

        public static int GetCurrentMonitorIndex()
        {
            GetCursorPos(out POINT pt);
            IntPtr currentMonitor = MonitorFromPoint(pt, MONITOR_DEFAULT_TO_NEAREST);

            int index = -1;
            int counter = 0;

            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
                (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData) =>
                {
                    if (hMonitor == currentMonitor)
                    {
                        index = counter;
                    }
                    counter++;
                    return true; // continue enumeration
                },
                IntPtr.Zero);

            return index;
        }
    }
}

#elif UNITY_EDITOR && UNITY_EDITOR_OSX
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Tools
{
    public static class UniversalMousePosition
    {
        private const string CoreGraphics = "/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics";

        [StructLayout(LayoutKind.Sequential)]
        private struct CGPoint { public double x; public double y; }

        [StructLayout(LayoutKind.Sequential)]
        private struct CGSize { public double width; public double height; }

        [StructLayout(LayoutKind.Sequential)]
        private struct CGRect 
        { 
            public CGPoint origin; 
            public CGSize size; 
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
            public int Width => Right - Left;
            public int Height => Bottom - Top;

            public override string ToString() => $"Left: {Left}, Top: {Top}, Right: {Right}, Bottom: {Bottom}";
        }

        [DllImport(CoreGraphics)]
        private static extern IntPtr CGEventCreate(IntPtr source);
        [DllImport(CoreGraphics)]
        private static extern CGPoint CGEventGetLocation(IntPtr @event);
        [DllImport(CoreGraphics)]
        private static extern void CFRelease(IntPtr cfTypeRef);
        [DllImport(CoreGraphics)]
        private static extern int CGGetDisplaysWithPoint(CGPoint point, uint maxDisplays, [Out] uint[] displays, out uint displayCount);
        [DllImport(CoreGraphics)]
        private static extern int CGGetActiveDisplayList(uint maxDisplays, [Out] uint[] displays, out uint displayCount);
        [DllImport(CoreGraphics)]
        private static extern CGRect CGDisplayBounds(uint display);

        private static bool TryGetMouseGlobal(out CGPoint pt)
        {
            IntPtr ev = CGEventCreate(IntPtr.Zero);
            if (ev == IntPtr.Zero) { pt = default; return false; }
            pt = CGEventGetLocation(ev);
            CFRelease(ev);
            return true;
        }

        public static Vector2Int GetCursorPosition()
        {
            if (!TryGetMouseGlobal(out CGPoint gpt)) return Vector2Int.zero;
            return new Vector2Int((int)Math.Round(gpt.x), (int)Math.Round(gpt.y));
        }

        public static RECT GetCurrentMonitorRect()
        {
            if (TryGetMouseGlobal(out CGPoint gpt))
            {
                uint[] ids = new uint[1];
                if (CGGetDisplaysWithPoint(gpt, 1, ids, out uint count) == 0 && count > 0)
                {
                    CGRect bounds = CGDisplayBounds(ids[0]);
                    return new RECT
                    {
                        Left = (int)bounds.origin.x,
                        Top = (int)bounds.origin.y,
                        Right = (int)(bounds.origin.x + bounds.size.width),
                        Bottom = (int)(bounds.origin.y + bounds.size.height)
                    };
                }
            }
            return new RECT();
        }

        public static int GetCurrentMonitorIndex()
        {
            if (!TryGetMouseGlobal(out CGPoint gpt)) return -1;

            uint[] activeIds = new uint[16];
            if (CGGetActiveDisplayList(16, activeIds, out uint totalCount) != 0) return -1;

            uint[] currentId = new uint[1];
            if (CGGetDisplaysWithPoint(gpt, 1, currentId, out uint foundCount) == 0 && foundCount > 0)
            {
                for (int i = 0; i < totalCount; i++)
                {
                    if (activeIds[i] == currentId[0]) return i;
                }
            }
            return -1;
        }
    }
}
#endif