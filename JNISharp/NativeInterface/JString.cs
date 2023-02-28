namespace JNISharp.NativeInterface;

public record JString : JObject
{
    public string GetString() => JNI.GetJStringString(this);
}