using System.Runtime.InteropServices;
using JNISharp.NativeInterface;

namespace JNISharp.ToolInterface;

public static unsafe partial class JVMTI
{
    internal static JVMTIEnv* Env;

    static JVMTI()
    {
        if (Env == null)
        {
            var err = JNI.VM->Functions->GetEnv(JNI.VM, out var env, (int)Version.V1);

            if (err != JNI.Result.Ok)
            {
                throw new Exception("Failed to initialize JVMTI");
            }

            if (env == IntPtr.Zero)
            {
                throw new Exception("Failed to initialize JVMTI");
            }

            Env = (JVMTIEnv*)env;
        }
    }

    internal static Dictionary<string, JClass> LoadedClassCache { get; set; } = new ();

    public static JClass GetLoadedClass(string sig)
    {
        if (LoadedClassCache.TryGetValue(sig, out var found))
        {
            return found;
        }

        foreach (var cls in GetLoadedClasses())
        {
            if (GetClassSignature(cls).Item1 == sig)
            {
                var global = JNI.NewGlobalRef<JClass>(cls);

                LoadedClassCache.Add(sig, global);
                return global;
            }
        }

        return null;
    }

    public static IEnumerable<JClass> GetLoadedClasses()
    {
        var err = Env->Functions->GetLoadedClasses(Env, out var length, out var arrayHandle);

        if (err != Error.None)
        {
            throw new JVMTIErrorException(err);
        }

        return new JVMTIArray<JClass>(arrayHandle, length);
    }

    public static Tuple<string, string> GetClassSignature(JClass cls)
    {
        var err = Env->Functions->GetClassSignature(Env, cls.Handle, out var sigPtr, out var genericPtr);

        if (err != Error.None && err != Error.ClassNotPrepared)
        {
            throw new JVMTIErrorException(err);
        }

        string sigString = null;
        string genericString = null;

        if (sigPtr != IntPtr.Zero)
        {
            sigString = Marshal.PtrToStringAnsi(sigPtr);

            if (genericPtr != IntPtr.Zero)
            {
                genericString = Marshal.PtrToStringAnsi(genericPtr);
            }
        }

        Deallocate(sigPtr);
        Deallocate(genericPtr);

        return new Tuple<string, string>(sigString, genericString);
    }

    public static JClassSignature GetSignature(this JClass cls)
    {
        var err = Env->Functions->GetClassSignature(Env, cls.Handle, out var sigPtr, out var genericPtr);

        if (err != Error.None && err != Error.ClassNotPrepared)
        {
            throw new JVMTIErrorException(err);
        }

        string sigString = null;
        string genericString = null;

        if (sigPtr != IntPtr.Zero)
        {
            sigString = Marshal.PtrToStringAnsi(sigPtr);

            if (genericPtr != IntPtr.Zero)
            {
                genericString = Marshal.PtrToStringAnsi(genericPtr);
            }
        }

        Deallocate(sigPtr);
        Deallocate(genericPtr);

        return new JClassSignature(
            sigString,
            genericString,
            cls.GetFields().Select(f => f.GetSignature(cls)),
            cls.GetMethods().Select(m => m.GetSignature()));
    }

    public static IEnumerable<JMethodID> GetMethods(this JClass cls)
    {
        var err = Env->Functions->GetClassMethods(Env, cls.Handle, out var length, out var handle);

        if (err != Error.None && err != Error.ClassNotPrepared)
        {
            throw new JVMTIErrorException(err);
        }

        return new JVMTIArray<JMethodID>(handle, length);
    }

    public static IEnumerable<JFieldID> GetFields(this JClass cls)
    {
        var err = Env->Functions->GetClassFields(Env, cls.Handle, out var length, out var handle);

        if (err != Error.None && err != Error.ClassNotPrepared)
        {
            throw new JVMTIErrorException(err);
        }

        return new JVMTIArray<JFieldID>(handle, length);
    }

    public static JMethodSignature GetSignature(this JMethodID methodID)
    {
        var err = Env->Functions->GetMethodName(Env, methodID, out var namePtr, out var sigPtr, out var genericPtr);

        if (err != Error.None && err != Error.ClassNotPrepared)
        {
            throw new JVMTIErrorException(err);
        }

        string name = null;
        string sig = null;
        string generic = null;

        if (namePtr != IntPtr.Zero)
        {
            name = Marshal.PtrToStringAnsi(namePtr);

            if (sigPtr != IntPtr.Zero)
            {
                sig = Marshal.PtrToStringAnsi(sigPtr);

                if (genericPtr != IntPtr.Zero)
                {
                    generic = Marshal.PtrToStringAnsi(genericPtr);
                }
            }
        }

        Deallocate(namePtr);
        Deallocate(sigPtr);
        Deallocate(genericPtr);

        err = Env->Functions->GetMethodModifiers(Env, methodID, out var modifier);

        if (err != Error.None)
        {
            throw new Exception($"Failed to get JMethodID modifier. Error: {err}");
        }


        return new JMethodSignature(name, sig, generic, (JMethodAccessFlags)modifier);
    }

    public static JFieldSignature GetSignature(this JFieldID fieldID, JClass cls)
    {
        var err = Env->Functions->GetFieldName(Env, cls.Handle, fieldID, out var namePtr, out var sigPtr, out var genericPtr);

        if (err != Error.None && err != Error.ClassNotPrepared)
        {
            throw new JVMTIErrorException(err);
        }

        string name = null;
        string sig = null;
        string generic = null;

        if (namePtr != IntPtr.Zero)
        {
            name = Marshal.PtrToStringAnsi(namePtr);
            Deallocate(namePtr);

            if (sigPtr != IntPtr.Zero)
            {
                sig = Marshal.PtrToStringAnsi(sigPtr);
                Deallocate(sigPtr);

                if (genericPtr != IntPtr.Zero)
                {
                    generic = Marshal.PtrToStringAnsi(genericPtr);
                    Deallocate(genericPtr);
                }
            }
        }

        err = Env->Functions->GetFieldModifiers(Env, cls.Handle, fieldID, out var flags);

        if (err != Error.None && err != Error.ClassNotPrepared)
        {
            throw new JVMTIErrorException(err);
        }

        return new JFieldSignature(null, sig, generic, (JFieldAccessFlags)flags);
    }

    public static void Deallocate(IntPtr address)
    {
        Env->Functions->Deallocate(Env, address);
    }
}