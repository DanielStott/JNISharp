using System.Collections;

namespace JNISharp.NativeInterface;

public record JObjectArray<T> : JObject, IEnumerable<T> where T : JObject, new()
{
    public int Length => JNI.GetArrayLength(this);

    public T this[int index]
    {
        get => JNI.GetObjectArrayElement(this, index);
        set => JNI.SetObjectArrayElement(this, index, value);
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < Length; i++)
        {
            yield return JNI.GetObjectArrayElement(this, i);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void SetElement(T value, int index)
    {
        JNI.SetObjectArrayElement(this, index, value);
    }
}