namespace JNISharp.NativeInterface;

public class JThrowableException : Exception
{
    public JThrowableException()
    {
    }

    public JThrowableException(JThrowable throwable)
    {
        Throwable = throwable;
    }

    public JThrowable Throwable { get; init; }
}