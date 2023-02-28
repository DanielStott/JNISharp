using System.Collections;
using JNISharp.NativeInterface;

namespace JNISharp.ToolInterface;

internal class JVMTIArray<T> : IDisposable, IEnumerable<T>
{
    internal JVMTIArray(IntPtr handle, int length)
    {
        unsafe
        {
            Handle = handle;
            var arr = (IntPtr*)handle;
            var t = typeof(T);

            if (t == typeof(JMethodID))
            {
                var buffer = new JMethodID[length];

                for (var i = 0; i < length; i++)
                {
                    buffer[i] = arr[i];
                }

                Elements = (T[])(object)buffer;
            }
            else if (t == typeof(JFieldID))
            {
                var buffer = new JFieldID[length];

                for (var i = 0; i < length; i++)
                {
                    buffer[i] = arr[i];
                }

                Elements = (T[])(object)buffer;
            }
            else if (t == typeof(JClass))
            {
                var buffer = new JClass[length];

                for (var i = 0; i < length; i++)
                {
                    buffer[i] = new JClass { Handle = arr[i], ReferenceType = JNI.ReferenceType.Local };
                }

                Elements = (T[])(object)buffer;
            }
        }
    }

    private bool Disposed { get; set; }

    private IntPtr Handle { get; }

    private T[] Elements { get; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public IEnumerator<T> GetEnumerator()
    {
        //for (int i = 0; i < this.Elements.Length; i++)
        //yield return this.Elements[i];
        return ((IEnumerable<T>)Elements).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (Disposed)
        {
            return;
        }

        JVMTI.Deallocate(Handle);

        Disposed = true;
    }

    ~JVMTIArray()
    {
        Dispose(false);
    }
}