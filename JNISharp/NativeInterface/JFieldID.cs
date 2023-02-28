namespace JNISharp.NativeInterface;

public readonly struct JFieldID : IEquatable<JFieldID>
{
    public readonly IntPtr Handle { get; init; }

    internal JFieldID(IntPtr handle)
    {
        Handle = handle;
    }

    public static implicit operator IntPtr(JFieldID fieldID) => fieldID.Handle;

    public static implicit operator JFieldID(IntPtr pointer) => new (pointer);

    public bool Equals(JFieldID other) => Handle == other.Handle;
}