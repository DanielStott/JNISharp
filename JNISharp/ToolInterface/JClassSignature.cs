using System.Text.Json;
using System.Text.Json.Serialization;

namespace JNISharp.ToolInterface;

public class JClassSignature
{
    public JClassSignature(string signature, string generic, IEnumerable<JFieldSignature> fieldSignatures, IEnumerable<JMethodSignature> methodSignatures)
    {
        Signature = signature;
        Generic = generic;
        FieldSignatures = fieldSignatures;
        MethodSignatures = methodSignatures;
    }

    [JsonInclude]
    public string Signature { get; init; }

    [JsonInclude]
    public string Generic { get; init; }

    [JsonInclude]
    public IEnumerable<JFieldSignature> FieldSignatures { get; init; }

    [JsonInclude]
    public IEnumerable<JMethodSignature> MethodSignatures { get; init; }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public string ToJson(JsonSerializerOptions options)
    {
        return JsonSerializer.Serialize(this, options);
    }
}