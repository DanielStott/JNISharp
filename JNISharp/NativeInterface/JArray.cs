using System.Collections;

namespace JNISharp.NativeInterface;

public record JArray<T> : JObject, IEnumerable<T>
{
    public JArray()
    {
    }

    public JArray(int size)
    {
        JNI.NewArray<T>(size);
    }

    public int Length => JNI.GetArrayLength(this);

    public T this[int index]
    {
        get => JNI.GetArrayElement(this, index);
        set => JNI.SetArrayElement(this, index, value);
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < Length; i++)
            yield return JNI.GetArrayElement(this, i);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public T[] GetElements() => JNI.GetArrayElements(this);
}