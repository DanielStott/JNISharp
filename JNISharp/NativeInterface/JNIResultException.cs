namespace JNISharp.NativeInterface;

public class JNIResultException : Exception
{
    public JNIResultException(JNI.Result result) : base($"JNI error occurred: {result}")
    {
        Result = result;
    }

    public JNI.Result Result { get; init; }
}