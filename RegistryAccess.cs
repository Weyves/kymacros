using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kymacros
{
    static internal class RegistryAccess
    {
        static internal RegistryKey GetDeviceKey(string device)
        {
            var split = device.Substring(4).Split('#');

            var classCode = split[0];       // Class code
            var subClassCode = split[1];    // SubClass code
            var protocolCode = split[2];    // Protocol code

            return Registry.LocalMachine.OpenSubKey(string.Format(@"System\CurrentControlSet\Enum\{0}\{1}\{2}", classCode, subClassCode, protocolCode));
        }

        static internal string GetClassType(string classGuid)
        {
            var classGuidKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\" + classGuid);

            return classGuidKey != null ? (string)classGuidKey.GetValue("Class") : string.Empty;
        }
    }
}
