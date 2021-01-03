using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Kymacros
{
    static public class RawInputHandler
    {
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool RegisterRawInputDevices(RawInputDevice[] pRawInputDevice, uint numDevices, uint size);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint GetRawInputDeviceList(IntPtr pRawInputDeviceList, ref uint numDevices, uint size);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint GetRawInputDeviceInfo(IntPtr hDevice, uint uiCommand, IntPtr pData, ref uint pcbSize);
    }
}
