using System.Runtime.InteropServices;

namespace JNISharp.NativeInterface;

[StructLayout(LayoutKind.Sequential)]
public struct JavaVMOption
{
    public IntPtr OptionString;

    public IntPtr ExtraInfo;

    public JavaVMOption(string optionString)
    {
        OptionString = Marshal.StringToHGlobalAnsi(optionString);
        ExtraInfo = IntPtr.Zero;
    }
}