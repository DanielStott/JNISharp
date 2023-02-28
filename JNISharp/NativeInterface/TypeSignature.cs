namespace JNISharp.NativeInterface;

public static partial class JNI
{
    public static string GetTypeSignature<T>()
    {
        var t = typeof(T);

        if (t == typeof(bool))
            return TypeSignature.Bool;

        if (t == typeof(sbyte))
            return TypeSignature.Byte;

        if (t == typeof(char))
            return TypeSignature.Char;

        if (t == typeof(short))
            return TypeSignature.Short;

        if (t == typeof(int))
            return TypeSignature.Int;

        if (t == typeof(long))
            return TypeSignature.Long;

        if (t == typeof(float))
            return TypeSignature.Float;

        if (t == typeof(double))
            return TypeSignature.Double;

        throw new ArgumentException($"GetTypeSignature Type {t} not supported.");
    }

    public static class TypeSignature
    {
        public const string Bool = "Z";

        public const string Byte = "B";

        public const string Char = "C";

        public const string Short = "S";

        public const string Int = "I";

        public const string Long = "J";

        public const string Float = "F";

        public const string Double = "D";
    }
}