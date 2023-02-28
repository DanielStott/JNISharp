namespace JNISharp.NativeInterface;

public record JThrowable : JObject
{
    public string GetMessage()
    {
        return JNI.FindClass("java/lang/Throwable").CallObjectMethod<JString>(this, "getMessage", "()Ljava/lang/String;").GetString();
    }

    public override string ToString()
    {
        return JNI.FindClass("java/lang/Throwable").CallObjectMethod<JString>(this, "toString", "()Ljava/lang/String;").GetString();
    }
}