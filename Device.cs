using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
internal struct Device
{
    internal string Name;
    internal string Description;
    internal uint IntType;
    internal IntPtr Handle;
    internal string Type;

    public override string ToString()
    {
        switch (IntType)
        {
            case 0:
                Type = "Mouse";
                break;
            case 1:
                Type = "Keyboard";
                break;
            case 2:
                Type = "HID";
                break;
        }
        return string.Format("Device: {0}.\nDescription: {1}.\nType: {2}.\nHandle: {3}", Name, Description, Type, Handle);
    }

    public string getTypeString()
    {
        switch (IntType)
        {
            case 0:
                Type = "Mouse";
                break;
            case 1:
                Type = "Keyboard";
                break;
            case 2:
                Type = "HID";
                break;
        }
        return Type;
    }
}