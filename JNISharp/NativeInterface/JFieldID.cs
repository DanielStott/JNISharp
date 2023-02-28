namespace JNISharp.NativeInterface;

public readonly struct JFieldID : IEquatable<JFieldID>
{
    public readonly IntPtr Handle { get; init; }

    internal JFieldID(IntPtr handle)
    {
        Handle = handle;
    }

    public static implicit operator IntPtr(JFieldID fieldID)
    {
        return fieldID.Handle;
    }

    public static implicit operator JFieldID(IntPtr pointer)
    {
        return new JFieldID(pointer);
    }

    public bool Equals(JFieldID other)
    {
        return Handle == other.Handle;
    }
}