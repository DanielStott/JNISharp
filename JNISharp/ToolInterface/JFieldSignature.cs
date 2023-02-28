using System.Text.Json.Serialization;

namespace JNISharp.ToolInterface;

public class JFieldSignature
{
    public JFieldSignature(string name, string sig, string generic, JFieldAccessFlags flags)
    {
        Name = name;
        Signature = sig;
        Generic = generic;
        AccessFlags = flags;
    }

    [JsonInclude]
    public string Name { get; init; }

    [JsonInclude]
    public string Signature { get; init; }


    [JsonInclude]
    public string Generic { get; init; }

    [JsonInclude]
    public JFieldAccessFlags AccessFlags { get; init; }
}