using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ChanSlider
{
    static class NativeMethods
    {
        [DllImport("urlmon.dll", ExactSpelling = true, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        private static extern int ObtainUserAgentString(int dwOption, StringBuilder userAgent, ref int length);

        internal static string ObtainUserAgentString()
        {
            int length = 255;
            var userAgentBuffer = new StringBuilder(length);
            int hr = ObtainUserAgentString(0, userAgentBuffer, ref length);

            if (hr != 0)
                return string.Empty;

            return userAgentBuffer.ToString();
        }
    }
}
