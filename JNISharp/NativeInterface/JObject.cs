namespace JNISharp.NativeInterface;

public record JObject : IDisposable
{
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

    private bool Disposed { get; set; }

    public IntPtr Handle { get; init; }

    internal JNI.ReferenceType ReferenceType { get; init; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (Disposed || Handle == IntPtr.Zero)
        {
            return;
        }

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
        Dispose(false);
    }

    public override string ToString()
    {
        var methodId = JNI.GetObjectClass(this).GetMethodID("toString", "()Ljava/lang/String;");
        return JNI.CallObjectMethod<JString>(this, methodId).GetString();
    }
}