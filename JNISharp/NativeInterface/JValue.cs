using System.Runtime.InteropServices;

namespace JNISharp.NativeInterface;

[StructLayout(LayoutKind.Explicit, Size = 8)]
public readonly struct JValue
{
    [FieldOffset(0)] public readonly byte Z;

    [FieldOffset(0)] public readonly sbyte B;

    [FieldOffset(0)] public readonly char C;

    [FieldOffset(0)] public readonly short S;

    [FieldOffset(0)] public readonly int I;

    [FieldOffset(0)] public readonly long J;

    [FieldOffset(0)] public readonly float F;

    [FieldOffset(0)] public readonly double D;

    [FieldOffset(0)] public readonly IntPtr L;

    public JValue(bool value)
    {
        this = new JValue();
        Z = Convert.ToByte(value);
    }

    public JValue(sbyte value)
    {
        this = new JValue();
        B = value;
    }

    public JValue(char value)
    {
        this = new JValue();
        C = value;
    }

    public JValue(short value)
    {
        this = new JValue();
        S = value;
    }

    public JValue(int value)
    {
        this = new JValue();
        I = value;
    }

    public JValue(long value)
    {
        this = new JValue();
        J = value;
    }

    public JValue(float value)
    {
        this = new JValue();
        F = value;
    }

    public JValue(double value)
    {
        this = new JValue();
        D = value;
    }

    public JValue(IntPtr value)
    {
        this = new JValue();
        L = value;
    }

    public JValue(JObject obj)
    {
        this = new JValue();
        L = obj.Handle;
    }

    public JValue(object value)
    {
        this = new JValue();

        switch (value)
        {
            case bool z:
                Z = Convert.ToByte(z);
                break;

            case sbyte b:
                B = b;
                break;

            case char c:
                C = c;
                break;

            case short s:
                S = s;
                break;

            case int i:
                I = i;
                break;

            case long j:
                J = j;
                break;

            case float f:
                F = f;
                break;

            case double d:
                D = d;
                break;

            case JObject obj:
                L = obj.Handle;
                break;
        }
    }

    public static implicit operator JValue(bool b) => new (b);

    public static implicit operator JValue(sbyte b) => new (b);

    public static implicit operator JValue(char c) => new (c);

    public static implicit operator JValue(short s) => new (s);

    public static implicit operator JValue(int i) => new (i);

    public static implicit operator JValue(long j) => new (j);

    public static implicit operator JValue(float f) => new (f);

    public static implicit operator JValue(double d) => new (d);

    public static implicit operator JValue(JObject obj) => new (obj);
}