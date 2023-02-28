namespace JNISharp.NativeInterface;

public record JObject : IDisposable
{
    private bool Disposed { get; set; }

    public IntPtr Handle { get; init; }

    internal JNI.ReferenceType ReferenceType { get; init; }

    public JObject()
    {
    }

    public JObject(IntPtr handle, JNI.ReferenceType referenceType)
    {
        Handle = handle;
        ReferenceType = referenceType;
    }

    public JObject(JObject obj)
    {
        Handle = obj.Handle;
        ReferenceType = obj.ReferenceType;
        Disposed = obj.Disposed;
        obj.Disposed = true;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (Disposed || Handle == IntPtr.Zero)
            return;

        switch (ReferenceType)
        {
            case JNI.ReferenceType.Local:
                JNI.DeleteLocalRef(this);
                break;

            case JNI.ReferenceType.Global:
                JNI.DeleteGlobalRef(this);
                break;

            case JNI.ReferenceType.WeakGlobal:
                JNI.DeleteWeakGlobalRef(this);
                break;
        }

        Disposed = true;
    }

    public bool Valid()
    {
        return Handle != IntPtr.Zero;
    }

    ~JObject()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}