namespace JNISharp.ToolInterface;

public class JMethodSignature
{
    public JMethodSignature(string name, string sig, string generic, JMethodAccessFlags flags)
    {
        Name = name;
        Signature = sig;
        Generic = generic;
        AccessFlags = flags;
    }

    public string Name { get; init; }

    public string Signature { get; init; }

    public string Generic { get; init; }

    public JMethodAccessFlags AccessFlags { get; init; }
}