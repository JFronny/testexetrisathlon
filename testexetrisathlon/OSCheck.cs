using System;
using System.Linq;

namespace testexetrisathlon
{
    internal static class OSCheck
    {
        private static bool _checkedWindows;
        private static bool _isWindows;

        public static bool IsWindows
        {
            get
            {
                if (_checkedWindows) return _isWindows;
                _isWindows =
                    new[] {PlatformID.Win32S, PlatformID.Win32Windows, PlatformID.Win32NT, PlatformID.WinCE}
                        .Contains(Environment.OSVersion.Platform);
                _checkedWindows = true;
                return _isWindows;
            }
        }
    }
}