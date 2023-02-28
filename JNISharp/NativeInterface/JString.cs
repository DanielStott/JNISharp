namespace JNISharp.NativeInterface;

public record JString : JObject
{
    public string GetString()
    {
        return JNI.GetJStringString(this);
    }
}