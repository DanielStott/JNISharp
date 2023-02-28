namespace JNISharp.NativeInterface;

public record JString : JObject
{
    public JString() : base()
    {
    }

    public string GetString() => JNI.GetJStringString(this);
}