namespace JNISharp.NativeInterface;

public readonly struct JMethodID : IEquatable<JMethodID>
{
    public readonly IntPtr Handle { get; init; }

    internal JMethodID(IntPtr handle)
    {
        Handle = handle;
    }

    public static implicit operator IntPtr(JMethodID methodID)
    {
        return methodID.Handle;
    }

    public static implicit operator JMethodID(IntPtr pointer)
    {
        return new JMethodID(pointer);
    }

    public bool Equals(JMethodID other)
    {
        return Handle == other.Handle;
    }
}