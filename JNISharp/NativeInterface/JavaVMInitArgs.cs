using System.Runtime.InteropServices;

namespace JNISharp.NativeInterface;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct JavaVMInitArgs
{
    public int Version;

    public int OptionsCount;

    public JavaVMOption* Options;

    public bool IgnoreUnrecognized;

    public JavaVMInitArgs(JNI.Version version, JavaVMOption[] options, bool ignoreUnrecognized)
    {
        Version = (int)version;
        OptionsCount = options.Length;
        Options = (JavaVMOption*)Marshal.UnsafeAddrOfPinnedArrayElement(options, 0);
        IgnoreUnrecognized = ignoreUnrecognized;
    }
}