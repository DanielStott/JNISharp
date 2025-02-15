﻿using System.Runtime.InteropServices;

namespace JNISharp.NativeInterface;

/// <summary>
///     Represents the Java Native Interface
/// </summary>
public static unsafe partial class JNI
{
    internal static JavaVM* VM;

    [ThreadStatic] internal static JNIEnv* env;

    internal static JNIEnv* Env
    {
        get
        {
            if (env == null)
            {
                var temp = IntPtr.Zero;
                var res = VM->Functions->AttachCurrentThread(VM, out env, ref temp);

                if (res != Result.Ok)
                {
                    throw new JNIResultException(res);
                }
            }

            return env;
        }
    }

    internal static Dictionary<string, JClass> ClassCache { get; set; } = new ();

    public static void Initialize(JavaVMInitArgs args)
    {
        var res = JVMImports.JNI_CreateJavaVM(out VM, out env, &args);

        if (res != Result.Ok)
        {
            throw new JNIResultException(res);
        }
    }

    public static void InitializeFromCreatedVM()
    {
        var res = JVMImports.JNI_GetCreatedJavaVMs(out VM, 1, out var _);

        if (res != Result.Ok)
        {
            throw new JNIResultException(res);
        }

        var temp = IntPtr.Zero;
        res = VM->Functions->AttachCurrentThread(VM, out env, ref temp);

        if (res != Result.Ok)
        {
            throw new JNIResultException(res);
        }
    }

    public static int GetVersion()
    {
        return Env->Functions->GetVersion(Env);
    }

    public static JClass DefineClass(string name, JObject loader, sbyte[] bytes)
    {
        var bytesPtr = Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0);
        var nameAnsi = Marshal.StringToHGlobalAnsi(name);

        var res = Env->Functions->DefineClass(Env, nameAnsi, loader.Handle, bytesPtr, bytes.Length);

        Marshal.FreeHGlobal(nameAnsi);

        using JClass local = new () { Handle = res, ReferenceType = ReferenceType.Local };
        return NewGlobalRef<JClass>(local);
    }

    public static JClass FindClass(string name)
    {
        if (ClassCache.TryGetValue(name, out var found))
        {
            return found;
        }

        var nameAnsi = Marshal.StringToHGlobalAnsi(name);
        var res = Env->Functions->FindClass(Env, nameAnsi);

        Marshal.FreeHGlobal(nameAnsi);

        using JClass local = new () { Handle = res, ReferenceType = ReferenceType.Local };
        var global = NewGlobalRef<JClass>(local);
        ClassCache.Add(name, global);
        return global;
    }

    public static JMethodID FromReflectedMethod(JObject method)
    {
        return Env->Functions->FromReflectedMethod(Env, method.Handle);
    }

    public static JFieldID FromReflectedField(JObject field)
    {
        return Env->Functions->FromReflectedField(Env, field.Handle);
    }

    public static JObject ToReflectedMethod(JClass cls, JMethodID methodID, bool isStatic)
    {
        var res = Env->Functions->ToReflectedMethod(Env, cls.Handle, methodID, Convert.ToByte(isStatic));

        using JObject local = new () { Handle = res, ReferenceType = ReferenceType.Local };
        return NewGlobalRef<JObject>(local);
    }

    public static JClass GetSuperClass(JClass sub)
    {
        var res = Env->Functions->GetSuperClass(Env, sub.Handle);

        using JClass local = new () { Handle = res, ReferenceType = ReferenceType.Local };
        return NewGlobalRef<JClass>(local);
    }

    public static bool IsAssignableFrom(JClass sub, JClass sup)
    {
        return Convert.ToBoolean(Env->Functions->IsAssignableFrom(Env, sub.Handle, sup.Handle));
    }

    public static JObject ToReflectedField(JClass cls, JFieldID fieldID, bool isStatic)
    {
        throw new NotImplementedException();
    }

    public static void Throw(JThrowable throwable)
    {
        var res = Env->Functions->Throw(Env, throwable.Handle);
    }

    public static void ThrowNew(JClass cls, string message)
    {
        var messageAnsi = Marshal.StringToHGlobalAnsi(message);
        var res = Env->Functions->Throw(Env, messageAnsi);
        Marshal.FreeHGlobal(messageAnsi);
    }

    public static JThrowable ExceptionOccurred()
    {
        var res = Env->Functions->ExceptionOccurred(Env);

        using JThrowable local = new () { Handle = res, ReferenceType = ReferenceType.Local };
        return NewGlobalRef<JThrowable>(local);
    }

    public static void ExceptionDescribe()
    {
        Env->Functions->ExceptionDescribe(Env);
    }

    public static void ExceptionClear()
    {
        Env->Functions->ExceptionClear(Env);
    }

    public static void FatalError(string message)
    {
        throw new NotImplementedException();
    }

    public static int PushLocalFrame(int capacity)
    {
        throw new NotImplementedException();
    }

    public static JObject PopLocalFrame(JObject result)
    {
        throw new NotImplementedException();
    }

    public static T NewGlobalRef<T>(JObject lobj) where T : JObject, new()
    {
        var res = Env->Functions->NewGlobalRef(Env, lobj.Handle);
        return new T { Handle = res, ReferenceType = ReferenceType.Global };
    }

    public static void DeleteGlobalRef(JObject gref)
    {
        if (gref == null)
        {
            return;
        }

        if (!gref.Valid())
        {
            return;
        }

        Env->Functions->DeleteGlobalRef(Env, gref.Handle);
    }

    public static void CheckExceptionAndThrow()
    {
        if (ExceptionCheck())
        {
            var throwable = ExceptionOccurred();
            ExceptionClear();
            throw new JThrowableException(throwable);
        }
    }

    public static void DeleteLocalRef(JObject lref)
    {
        if (lref == null)
        {
            return;
        }

        if (!lref.Valid())
        {
            return;
        }

        Env->Functions->DeleteLocalRef(Env, lref.Handle);
    }

    public static bool IsSameObject(JObject obj1, JObject obj2)
    {
        return Convert.ToBoolean(Env->Functions->IsSameObject(Env, obj1.Handle, obj2.Handle));
    }

    public static T NewLocalRef<T>(JObject obj) where T : JObject, new()
    {
        var res = Env->Functions->NewLocalRef(Env, obj.Handle);
        return new T { Handle = res, ReferenceType = ReferenceType.Local };
    }

    public static int EnsureLocalCapacity(int capacity)
    {
        return Env->Functions->EnsureLocalCapacity(Env, capacity);
    }

    public static T AllocObject<T>(JClass cls) where T : JObject, new()
    {
        var res = Env->Functions->AllocObject(Env, cls.Handle);

        using JObject local = new () { Handle = res, ReferenceType = ReferenceType.Local };
        return NewGlobalRef<T>(local);
    }

    public static T NewObject<T>(JClass cls, JMethodID methodID, params JValue[] args) where T : JObject, new()
    {
        var argsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(args, 0);
        var res = Env->Functions->NewObjectA(Env, cls.Handle, methodID, argsPtr);
        Console.WriteLine($"Res: {res}");
        JObject local = new () { Handle = res, ReferenceType = ReferenceType.Local };
        return NewGlobalRef<T>(local);
    }

    public static JClass GetObjectClass(JObject obj)
    {
        var res = Env->Functions->GetObjectClass(Env, obj.Handle);

        using JClass local = new () { Handle = res, ReferenceType = ReferenceType.Local };
        return NewGlobalRef<JClass>(local);
    }

    public static bool IsInstanceOf(JObject obj, JClass cls)
    {
        return Convert.ToBoolean(Env->Functions->IsInstanceOf(Env, obj.Handle, cls.Handle));
    }

    public static JMethodID GetMethodID(JClass cls, string name, string sig)
    {
        var nameAnsi = Marshal.StringToHGlobalAnsi(name);
        var sigAnsi = Marshal.StringToHGlobalAnsi(sig);

        JMethodID id = Env->Functions->GetMethodID(Env, cls.Handle, nameAnsi, sigAnsi);

        Marshal.FreeHGlobal(nameAnsi);
        Marshal.FreeHGlobal(sigAnsi);
        return id;
    }

    public static T CallObjectMethod<T>(JObject obj, JMethodID methodID, params JValue[] args) where T : JObject, new()
    {
        var argsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(args, 0);

        fixed (JValue* v = args)
        {
            var res = Env->Functions->CallObjectMethodA(Env, obj.Handle, methodID, argsPtr);
            using JObject local = new () { Handle = res, ReferenceType = ReferenceType.Local };
            return NewGlobalRef<T>(local);
        }
    }

    public static T CallMethod<T>(JObject obj, JMethodID methodID, params JValue[] args)
    {
        var t = typeof(T);
        var argsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(args, 0);

        if (t == typeof(bool))
        {
            return (T)(object)Convert.ToBoolean(Env->Functions->CallBooleanMethodA(Env, obj.Handle, methodID, argsPtr));
        }

        if (t == typeof(sbyte))
        {
            return (T)(object)Env->Functions->CallByteMethodA(Env, obj.Handle, methodID, argsPtr);
        }

        if (t == typeof(char))
        {
            return (T)(object)Env->Functions->CallCharMethodA(Env, obj.Handle, methodID, argsPtr);
        }

        if (t == typeof(short))
        {
            return (T)(object)Env->Functions->CallShortMethodA(Env, obj.Handle, methodID, argsPtr);
        }

        if (t == typeof(int))
        {
            return (T)(object)Env->Functions->CallIntMethodA(Env, obj.Handle, methodID, argsPtr);
        }

        if (t == typeof(long))
        {
            return (T)(object)Env->Functions->CallLongMethodA(Env, obj.Handle, methodID, argsPtr);
        }

        if (t == typeof(float))
        {
            return (T)(object)Env->Functions->CallFloatMethodA(Env, obj.Handle, methodID, argsPtr);
        }

        if (t == typeof(double))
        {
            return (T)(object)Env->Functions->CallDoubleMethodA(Env, obj.Handle, methodID, argsPtr);
        }

        throw new ArgumentException($"CallMethod Type {t} not supported.");
    }

    public static void CallVoidMethod(JObject obj, JMethodID methodID, params JValue[] args)
    {
        var argsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(args, 0);
        Env->Functions->CallVoidMethodA(Env, obj.Handle, methodID, argsPtr);
    }

    public static T CallNonvirtualObjectMethod<T>(JObject obj, JClass cls, JMethodID methodID, params JValue[] args) where T : JObject, new()
    {
        var argsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(args, 0);
        var res = Env->Functions->CallNonvirtualObjectMethodA(Env, obj.Handle, cls.Handle, methodID, argsPtr);

        using JObject local = new () { Handle = res, ReferenceType = ReferenceType.Local };
        return NewGlobalRef<T>(local);
    }

    public static T CallNonvirtualMethod<T>(JObject obj, JClass cls, JMethodID methodID, params JValue[] args)
    {
        var t = typeof(T);
        var argsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(args, 0);

        if (t == typeof(bool))
        {
            return (T)(object)Convert.ToBoolean(Env->Functions->CallNonvirtualBooleanMethodA(Env, obj.Handle, cls.Handle, methodID, argsPtr));
        }

        if (t == typeof(sbyte))
        {
            return (T)(object)Env->Functions->CallNonvirtualByteMethodA(Env, obj.Handle, cls.Handle, methodID, argsPtr);
        }

        if (t == typeof(char))
        {
            return (T)(object)Env->Functions->CallNonvirtualCharMethodA(Env, obj.Handle, cls.Handle, methodID, argsPtr);
        }

        if (t == typeof(short))
        {
            return (T)(object)Env->Functions->CallNonvirtualShortMethodA(Env, obj.Handle, cls.Handle, methodID, argsPtr);
        }

        if (t == typeof(int))
        {
            return (T)(object)Env->Functions->CallNonvirtualIntMethodA(Env, obj.Handle, cls.Handle, methodID, argsPtr);
        }

        if (t == typeof(long))
        {
            return (T)(object)Env->Functions->CallNonvirtualLongMethodA(Env, obj.Handle, cls.Handle, methodID, argsPtr);
        }

        if (t == typeof(float))
        {
            return (T)(object)Env->Functions->CallNonvirtualFloatMethodA(Env, obj.Handle, cls.Handle, methodID, argsPtr);
        }

        if (t == typeof(double))
        {
            return (T)(object)Env->Functions->CallNonvirtualDoubleMethodA(Env, obj.Handle, cls.Handle, methodID, argsPtr);
        }

        throw new ArgumentException($"CallNonvirtualMethod Type {t} not supported.");
    }

    public static void CallNonvirtualVoidMethod(JObject obj, JClass cls, JMethodID methodID, params JValue[] args)
    {
        var argsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(args, 0);
        Env->Functions->CallNonvirtualVoidMethodA(Env, obj.Handle, cls.Handle, methodID, argsPtr);
    }

    public static JFieldID GetFieldID(JClass cls, string name, string sig)
    {
        var nameAnsi = Marshal.StringToHGlobalAnsi(name);
        var sigAnsi = Marshal.StringToHGlobalAnsi(sig);

        JFieldID id = Env->Functions->GetFieldID(Env, cls.Handle, nameAnsi, sigAnsi);

        Marshal.FreeHGlobal(nameAnsi);
        Marshal.FreeHGlobal(sigAnsi);
        return id;
    }

    public static T GetObjectField<T>(JObject obj, JFieldID fieldID) where T : JObject, new()
    {
        var res = Env->Functions->GetObjectField(Env, obj.Handle, fieldID);
        using JObject local = new () { Handle = res, ReferenceType = ReferenceType.Local };
        return NewGlobalRef<T>(local);
    }

    public static T GetField<T>(JObject obj, JFieldID fieldID)
    {
        var t = typeof(T);

        if (t == typeof(bool))
        {
            return (T)(object)Convert.ToBoolean(Env->Functions->GetBooleanField(Env, obj.Handle, fieldID));
        }

        if (t == typeof(sbyte))
        {
            return (T)(object)Env->Functions->GetByteField(Env, obj.Handle, fieldID);
        }

        if (t == typeof(char))
        {
            return (T)(object)Env->Functions->GetCharField(Env, obj.Handle, fieldID);
        }

        if (t == typeof(short))
        {
            return (T)(object)Env->Functions->GetShortField(Env, obj.Handle, fieldID);
        }

        if (t == typeof(int))
        {
            return (T)(object)Env->Functions->GetIntField(Env, obj.Handle, fieldID);
        }

        if (t == typeof(long))
        {
            return (T)(object)Env->Functions->GetLongField(Env, obj.Handle, fieldID);
        }

        if (t == typeof(float))
        {
            return (T)(object)Env->Functions->GetFloatField(Env, obj.Handle, fieldID);
        }

        if (t == typeof(double))
        {
            return (T)(object)Env->Functions->GetDoubleField(Env, obj.Handle, fieldID);
        }

        throw new ArgumentException($"GetField Type {t} not supported.");
    }

    public static void SetObjectField(JObject obj, JFieldID fieldID, JObject val)
    {
        Env->Functions->SetObjectField(Env, obj.Handle, fieldID, val.Handle);
    }

    public static void SetField<T>(JObject obj, JFieldID fieldID, T value)
    {
        switch (value)
        {
            case bool b:
                Env->Functions->SetBooleanField(Env, obj.Handle, fieldID, Convert.ToByte(b));
                break;

            case sbyte b:
                Env->Functions->SetByteField(Env, obj.Handle, fieldID, b);
                break;

            case char c:
                Env->Functions->SetCharField(Env, obj.Handle, fieldID, c);
                break;

            case short s:
                Env->Functions->SetShortField(Env, obj.Handle, fieldID, s);
                break;

            case int i:
                Env->Functions->SetIntField(Env, obj.Handle, fieldID, i);
                break;

            case float f:
                Env->Functions->SetFloatField(Env, obj.Handle, fieldID, f);
                break;

            case double d:
                Env->Functions->SetDoubleField(Env, obj.Handle, fieldID, d);
                break;

            default:
                throw new ArgumentException($"SetField Type {value?.GetType()} not supported.");
        }
    }

    public static JMethodID GetStaticMethodID(JClass cls, string name, string sig)
    {
        var nameAnsi = Marshal.StringToHGlobalAnsi(name);
        var sigAnsi = Marshal.StringToHGlobalAnsi(sig);
        JMethodID id = Env->Functions->GetStaticMethodID(Env, cls.Handle, nameAnsi, sigAnsi);
        return id;
    }

    public static T CallStaticObjectMethod<T>(JClass cls, JMethodID methodID, params JValue[] args) where T : JObject, new()
    {
        var argsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(args, 0);
        var res = Env->Functions->CallStaticObjectMethodA(Env, cls.Handle, methodID.Handle, argsPtr);
        using JObject local = new () { Handle = res, ReferenceType = ReferenceType.Local };
        return NewGlobalRef<T>(local);
    }

    public static T CallStaticMethod<T>(JClass cls, JMethodID methodID, params JValue[] args)
    {
        var t = typeof(T);
        var argsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(args, 0);

        if (t == typeof(bool))
        {
            return (T)(object)Convert.ToBoolean(Env->Functions->CallStaticBooleanMethodA(Env, cls.Handle, methodID, argsPtr));
        }

        if (t == typeof(sbyte))
        {
            return (T)(object)Env->Functions->CallStaticByteMethodA(Env, cls.Handle, methodID, argsPtr);
        }

        if (t == typeof(char))
        {
            return (T)(object)Env->Functions->CallStaticCharMethodA(Env, cls.Handle, methodID, argsPtr);
        }

        if (t == typeof(short))
        {
            return (T)(object)Env->Functions->CallStaticShortMethodA(Env, cls.Handle, methodID, argsPtr);
        }

        if (t == typeof(int))
        {
            return (T)(object)Env->Functions->CallStaticIntMethodA(Env, cls.Handle, methodID, argsPtr);
        }

        if (t == typeof(long))
        {
            return (T)(object)Env->Functions->CallStaticLongMethodA(Env, cls.Handle, methodID, argsPtr);
        }

        if (t == typeof(float))
        {
            return (T)(object)Env->Functions->CallStaticFloatMethodA(Env, cls.Handle, methodID, argsPtr);
        }

        if (t == typeof(double))
        {
            return (T)(object)Env->Functions->CallStaticDoubleMethodA(Env, cls.Handle, methodID, argsPtr);
        }

        throw new ArgumentException($"CallStaticMethod Type {t} not supported.");
    }

    public static void CallStaticVoidMethod(JClass cls, JMethodID methodID, params JValue[] args)
    {
        var argsPtr = Marshal.UnsafeAddrOfPinnedArrayElement(args, 0);
        Env->Functions->CallStaticVoidMethodA(Env, cls.Handle, methodID, argsPtr);
    }

    public static JFieldID GetStaticFieldID(JClass cls, string name, string sig)
    {
        var nameAnsi = Marshal.StringToHGlobalAnsi(name);
        var sigAnsi = Marshal.StringToHGlobalAnsi(sig);

        JFieldID id = Env->Functions->GetStaticFieldID(Env, cls.Handle, nameAnsi, sigAnsi);

        Marshal.FreeHGlobal(nameAnsi);
        Marshal.FreeHGlobal(sigAnsi);
        return id;
    }

    public static T GetStaticObjectField<T>(JClass cls, JFieldID fieldID) where T : JObject, new()
    {
        var res = Env->Functions->GetStaticObjectField(Env, cls.Handle, fieldID);

        using JObject local = new () { Handle = res, ReferenceType = ReferenceType.Local };
        return NewGlobalRef<T>(local);
    }

    public static T GetStaticField<T>(JClass cls, JFieldID fieldID)
    {
        var t = typeof(T);

        if (t == typeof(bool))
        {
            return (T)(object)Convert.ToBoolean(Env->Functions->GetStaticBooleanField(Env, cls.Handle, fieldID));
        }

        if (t == typeof(sbyte))
        {
            return (T)(object)Env->Functions->GetStaticByteField(Env, cls.Handle, fieldID);
        }

        if (t == typeof(char))
        {
            return (T)(object)Env->Functions->GetStaticCharField(Env, cls.Handle, fieldID);
        }

        if (t == typeof(short))
        {
            return (T)(object)Env->Functions->GetStaticShortField(Env, cls.Handle, fieldID);
        }

        if (t == typeof(int))
        {
            return (T)(object)Env->Functions->GetStaticIntField(Env, cls.Handle, fieldID);
        }

        if (t == typeof(long))
        {
            return (T)(object)Env->Functions->GetStaticLongField(Env, cls.Handle, fieldID);
        }

        if (t == typeof(float))
        {
            return (T)(object)Env->Functions->GetStaticFloatField(Env, cls.Handle, fieldID);
        }

        if (t == typeof(double))
        {
            return (T)(object)Env->Functions->GetStaticDoubleField(Env, cls.Handle, fieldID);
        }

        throw new ArgumentException($"GetStaticField Type {t} not supported.");
    }

    public static void SetStaticObjectField<T>(JClass cls, JFieldID fieldID, T value) where T : JObject, new()
    {
        Env->Functions->SetStaticObjectField(Env, cls.Handle, fieldID, value.Handle);
    }

    public static void SetStaticField<T>(JClass cls, JFieldID fieldID, T value)
    {
        switch (value)
        {
            case bool b:
                Env->Functions->SetStaticBooleanField(Env, cls.Handle, fieldID, Convert.ToByte(b));
                break;

            case sbyte b:
                Env->Functions->SetStaticByteField(Env, cls.Handle, fieldID, b);
                break;

            case char c:
                Env->Functions->SetStaticCharField(Env, cls.Handle, fieldID, c);
                break;

            case short s:
                Env->Functions->SetStaticShortField(Env, cls.Handle, fieldID, s);
                break;

            case int i:
                Env->Functions->SetStaticIntField(Env, cls.Handle, fieldID, i);
                break;

            case float f:
                Env->Functions->SetStaticFloatField(Env, cls.Handle, fieldID, f);
                break;

            case double d:
                Env->Functions->SetStaticDoubleField(Env, cls.Handle, fieldID, d);
                break;

            default:
                throw new ArgumentException($"SetField Type {value.GetType()} not supported.");
        }
    }

    public static JString NewString(string str)
    {
        var strUni = Marshal.StringToHGlobalUni(str);

        var res = Env->Functions->NewString(Env, strUni, str.Length);

        Marshal.FreeHGlobal(strUni);

        using JObject local = new () { Handle = res, ReferenceType = ReferenceType.Local };
        return NewGlobalRef<JString>(local);
    }

    public static int GetStringLength(JString str)
    {
        return Env->Functions->GetStringLength(Env, str.Handle);
    }

    public static string GetJStringString(JString str)
    {
        if (!str.Valid())
        {
            return "";
        }

        var res = Env->Functions->GetStringChars(Env, str.Handle, out var isCopy);
        var resultString = Marshal.PtrToStringUni(res);
        Env->Functions->ReleaseStringChars(Env, str.Handle, res);
        return resultString ?? "";
    }

    private static void ReleaseStringChars(JString str, IntPtr chars)
    {
        throw new NotImplementedException();
    }

    public static int GetArrayLength<T>(JArray<T> jarray)
    {
        return Env->Functions->GetArrayLength(Env, jarray.Handle);
    }

    public static int GetArrayLength<T>(JObjectArray<T> jarray) where T : JObject, new()
    {
        return Env->Functions->GetArrayLength(Env, jarray.Handle);
    }

    public static T GetObjectArrayElement<T>(JObjectArray<T> array, int index) where T : JObject, new()
    {
        var res = Env->Functions->GetObjectArrayElement(Env, array.Handle, index);

        using JObject local = new () { Handle = res, ReferenceType = ReferenceType.Local };
        return NewGlobalRef<T>(local);
    }

    public static void SetObjectArrayElement<T>(JObjectArray<T> array, int index, T value) where T : JObject, new()
    {
        Env->Functions->SetObjectArrayElement(Env, array.Handle, index, value.Handle);
    }

    public static JArray<T> NewArray<T>(int length)
    {
        var t = typeof(T);
        IntPtr res;

        if (t == typeof(bool))
        {
            res = Env->Functions->NewBooleanArray(Env, length);
        }
        else if (t == typeof(sbyte))
        {
            res = Env->Functions->NewByteArray(Env, length);
        }
        else if (t == typeof(char))
        {
            res = Env->Functions->NewCharArray(Env, length);
        }
        else if (t == typeof(short))
        {
            res = Env->Functions->NewShortArray(Env, length);
        }
        else if (t == typeof(int))
        {
            res = Env->Functions->NewIntArray(Env, length);
        }
        else if (t == typeof(long))
        {
            res = Env->Functions->NewBooleanArray(Env, length);
        }
        else if (t == typeof(float))
        {
            res = Env->Functions->NewBooleanArray(Env, length);
        }
        else if (t == typeof(double))
        {
            res = Env->Functions->NewBooleanArray(Env, length);
        }
        else
        {
            throw new ArgumentException($"CallStaticMethod Type {t} not supported.");
        }

        using JObject local = new () { Handle = res, ReferenceType = ReferenceType.Local };
        return NewGlobalRef<JArray<T>>(local);
    }

    public static T[] GetArrayElements<T>(JArray<T> array)
    {
        var t = typeof(T);
        var length = GetArrayLength(array);

        if (t == typeof(bool))
        {
            var arr = Env->Functions->GetBooleanArrayElements(Env, array.Handle, out var isCopy);

            var buf = new bool[length];

            for (var i = 0; i < length; i++)
            {
                buf[i] = Convert.ToBoolean(arr[i]);
            }

            Env->Functions->ReleaseBooleanArrayElements(Env, array.Handle, arr, (int)ReleaseMode.Abort);
            return (T[])(object)buf;
        }

        if (t == typeof(sbyte))
        {
            var arr = Env->Functions->GetByteArrayElements(Env, array.Handle, out var isCopy);

            var buf = new sbyte[length];

            for (var i = 0; i < length; i++)
            {
                buf[i] = arr[i];
            }

            Env->Functions->ReleaseByteArrayElements(Env, array.Handle, arr, (int)ReleaseMode.Abort);
            return (T[])(object)buf;
        }

        if (t == typeof(char))
        {
            var arr = Env->Functions->GetCharArrayElements(Env, array.Handle, out var isCopy);

            var buf = new char[length];

            for (var i = 0; i < length; i++)
            {
                buf[i] = arr[i];
            }

            Env->Functions->ReleaseCharArrayElements(Env, array.Handle, arr, (int)ReleaseMode.Abort);
            return (T[])(object)buf;
        }

        if (t == typeof(short))
        {
            var arr = Env->Functions->GetShortArrayElements(Env, array.Handle, out var isCopy);

            var buf = new short[length];

            for (var i = 0; i < length; i++)
            {
                buf[i] = arr[i];
            }

            Env->Functions->ReleaseShortArrayElements(Env, array.Handle, arr, (int)ReleaseMode.Abort);
            return (T[])(object)buf;
        }

        if (t == typeof(int))
        {
            var arr = Env->Functions->GetIntArrayElements(Env, array.Handle, out var isCopy);

            var buf = new int[length];

            for (var i = 0; i < length; i++)
            {
                buf[i] = arr[i];
            }

            Env->Functions->ReleaseIntArrayElements(Env, array.Handle, arr, (int)ReleaseMode.Abort);
            return (T[])(object)buf;
        }

        if (t == typeof(long))
        {
            var arr = Env->Functions->GetLongArrayElements(Env, array.Handle, out var isCopy);

            var buf = new long[length];

            for (var i = 0; i < length; i++)
            {
                buf[i] = arr[i];
            }

            Env->Functions->ReleaseLongArrayElements(Env, array.Handle, arr, (int)ReleaseMode.Abort);
            return (T[])(object)buf;
        }

        if (t == typeof(float))
        {
            var arr = Env->Functions->GetFloatArrayElements(Env, array.Handle, out var isCopy);

            var buf = new float[length];

            for (var i = 0; i < length; i++)
            {
                buf[i] = arr[i];
            }

            Env->Functions->ReleaseFloatArrayElements(Env, array.Handle, arr, (int)ReleaseMode.Abort);
            return (T[])(object)buf;
        }

        if (t == typeof(double))
        {
            var arr = Env->Functions->GetDoubleArrayElements(Env, array.Handle, out var isCopy);

            var buf = new double[length];

            for (var i = 0; i < length; i++)
            {
                buf[i] = arr[i];
            }

            Env->Functions->ReleaseDoubleArrayElements(Env, array.Handle, arr, (int)ReleaseMode.Abort);
            return (T[])(object)buf;
        }

        throw new ArgumentException($"GetArrayElements Type {t} not supported.");
    }

    public static T[] GetArrayRegion<T>(JArray<T> array, int start, int len)
    {
        var t = typeof(T);

        if (t == typeof(bool))
        {
            fixed (byte* buf = new byte[len])
            {
                Env->Functions->GetBooleanArrayRegion(Env, array.Handle, start, len, buf);

                var res = new bool[len];

                for (var i = 0; i < len; i++)
                {
                    res[i] = Convert.ToBoolean(buf[i]);
                }

                return (T[])(object)res;
            }
        }

        if (t == typeof(sbyte))
        {
            var buf = new sbyte[len];

            fixed (sbyte* b = buf)
            {
                Env->Functions->GetByteArrayRegion(Env, array.Handle, start, len, b);
                return (T[])(object)buf;
            }
        }

        if (t == typeof(char))
        {
            var buf = new char[len];

            fixed (char* b = buf)
            {
                Env->Functions->GetCharArrayRegion(Env, array.Handle, start, len, b);
                return (T[])(object)buf;
            }
        }

        if (t == typeof(short))
        {
            var buf = new short[len];

            fixed (short* b = buf)
            {
                Env->Functions->GetShortArrayRegion(Env, array.Handle, start, len, b);
                return (T[])(object)buf;
            }
        }

        if (t == typeof(int))
        {
            var buf = new int[len];

            fixed (int* b = buf)
            {
                Env->Functions->GetIntArrayRegion(Env, array.Handle, start, len, b);
                return (T[])(object)buf;
            }
        }

        if (t == typeof(long))
        {
            var buf = new long[len];

            fixed (long* b = buf)
            {
                Env->Functions->GetLongArrayRegion(Env, array.Handle, start, len, b);
                return (T[])(object)buf;
            }
        }

        if (t == typeof(float))
        {
            var buf = new float[len];

            fixed (float* b = buf)
            {
                Env->Functions->GetFloatArrayRegion(Env, array.Handle, start, len, b);
                return (T[])(object)buf;
            }
        }

        if (t == typeof(double))
        {
            var buf = new double[len];

            fixed (double* b = buf)
            {
                Env->Functions->GetDoubleArrayRegion(Env, array.Handle, start, len, b);
                return (T[])(object)buf;
            }
        }

        throw new ArgumentException($"GetArrayRegion Type {t} not supported.");
    }

    public static T GetArrayElement<T>(JArray<T> array, int index)
    {
        var t = typeof(T);

        if (t == typeof(bool))
        {
            byte b;
            Env->Functions->GetBooleanArrayRegion(Env, array.Handle, index, 1, &b);
            return (T)(object)Convert.ToBoolean(b);
        }

        if (t == typeof(sbyte))
        {
            sbyte b;
            Env->Functions->GetByteArrayRegion(Env, array.Handle, index, 1, &b);
            return (T)(object)b;
        }

        if (t == typeof(char))
        {
            char c;
            Env->Functions->GetCharArrayRegion(Env, array.Handle, index, 1, &c);
            return (T)(object)c;
        }

        if (t == typeof(short))
        {
            short s;
            Env->Functions->GetShortArrayRegion(Env, array.Handle, index, 1, &s);
            return (T)(object)s;
        }

        if (t == typeof(int))
        {
            int i;
            Env->Functions->GetIntArrayRegion(Env, array.Handle, index, 1, &i);
            return (T)(object)i;
        }

        if (t == typeof(long))
        {
            long l;
            Env->Functions->GetLongArrayRegion(Env, array.Handle, index, 1, &l);
            return (T)(object)l;
        }

        if (t == typeof(float))
        {
            float f;
            Env->Functions->GetFloatArrayRegion(Env, array.Handle, index, 1, &f);
            return (T)(object)f;
        }

        if (t == typeof(double))
        {
            double d;
            Env->Functions->GetDoubleArrayRegion(Env, array.Handle, index, 1, &d);
            return (T)(object)d;
        }

        throw new ArgumentException($"GetArrayElement Type {t} not supported.");
    }

    public static void SetArrayRegion<T>(JArray<T> array, int start, int len, T[] elems)
    {
        var t = typeof(T);

        if (t == typeof(bool))
        {
            fixed (byte* buf = elems.Select(b => Convert.ToByte(b)).ToArray())
            {
                Env->Functions->SetBooleanArrayRegion(Env, array.Handle, start, len, null);
            }
        }
        else if (t == typeof(sbyte))
        {
            fixed (sbyte* buf = (sbyte[])(object)elems)
            {
                Env->Functions->SetByteArrayRegion(Env, array.Handle, start, len, buf);
            }
        }
        else if (t == typeof(char))
        {
            fixed (char* buf = (char[])(object)elems)
            {
                Env->Functions->SetCharArrayRegion(Env, array.Handle, start, len, buf);
            }
        }
        else if (t == typeof(short))
        {
            fixed (short* buf = (short[])(object)elems)
            {
                Env->Functions->SetShortArrayRegion(Env, array.Handle, start, len, buf);
            }
        }
        else if (t == typeof(int))
        {
            fixed (int* buf = (int[])(object)elems)
            {
                Env->Functions->SetIntArrayRegion(Env, array.Handle, start, len, buf);
            }
        }
        else if (t == typeof(long))
        {
            fixed (long* buf = (long[])(object)elems)
            {
                Env->Functions->SetLongArrayRegion(Env, array.Handle, start, len, buf);
            }
        }
        else if (t == typeof(float))
        {
            fixed (float* buf = (float[])(object)elems)
            {
                Env->Functions->SetFloatArrayRegion(Env, array.Handle, start, len, buf);
            }
        }
        else if (t == typeof(double))
        {
            fixed (double* buf = (double[])(object)elems)
            {
                Env->Functions->SetDoubleArrayRegion(Env, array.Handle, start, len, buf);
            }
        }
        else
        {
            throw new ArgumentException($"SetArrayRegion Type {t} not supported.");
        }
    }

    public static void SetArrayElement<T>(JArray<T> array, int index, T value)
    {
        var t = typeof(T);

        if (t == typeof(bool))
        {
            var b = Convert.ToByte(value);
            Env->Functions->SetBooleanArrayRegion(Env, array.Handle, index, 1, &b);
        }
        else if (t == typeof(sbyte))
        {
            var b = (sbyte)(object)value;
            Env->Functions->SetByteArrayRegion(Env, array.Handle, index, 1, &b);
        }
        else if (t == typeof(char))
        {
            var c = (char)(object)value;
            Env->Functions->SetCharArrayRegion(Env, array.Handle, index, 1, &c);
        }
        else if (t == typeof(short))
        {
            var s = (short)(object)value;
            Env->Functions->SetShortArrayRegion(Env, array.Handle, index, 1, &s);
        }
        else if (t == typeof(int))
        {
            var c = (int)(object)value;
            Env->Functions->SetIntArrayRegion(Env, array.Handle, index, 1, &c);
        }
        else if (t == typeof(long))
        {
            var l = (long)(object)value;
            Env->Functions->SetLongArrayRegion(Env, array.Handle, index, 1, &l);
        }
        else if (t == typeof(float))
        {
            var f = (float)(object)value;
            Env->Functions->SetFloatArrayRegion(Env, array.Handle, index, 1, &f);
        }
        else if (t == typeof(double))
        {
            var d = (double)(object)value;
            Env->Functions->SetDoubleArrayRegion(Env, array.Handle, index, 1, &d);
        }
        else
        {
            throw new ArgumentException($"SetArrayElement Type {t} not supported.");
        }
    }

    private static int RegisterNatives(JClass cls, IntPtr methods, int nmethods)
    {
        throw new NotImplementedException();
    }

    private static int UnregisterNatives(JClass cls)
    {
        throw new NotImplementedException();
    }

    public static int MonitorEnter(JObject obj)
    {
        return Env->Functions->MonitorEnter(Env, obj.Handle);
    }

    public static int MonitorExit(JObject obj)
    {
        return Env->Functions->MonitorExit(Env, obj.Handle);
    }

    private static JavaVM* GetJavaVM()
    {
        throw new NotImplementedException();
    }

    public static string? GetStringRegion(JString str, int start, int len)
    {
        //Probably need to allocate space?
        var buf = IntPtr.Zero;
        Env->Functions->GetStringRegion(Env, str.Handle, start, len, buf);

        if (buf != IntPtr.Zero)
        {
            return Marshal.PtrToStringUni(buf);
        }

        return null;
    }

    private static IntPtr GetPrimitiveArrayCritical<T>(JArray<T> array)
    {
        throw new NotImplementedException();
    }

    private static void ReleasePrimitiveArrayCritical<T>(JArray<T> array, IntPtr carray, int mode)
    {
        throw new NotImplementedException();
    }

    private static string GetStringCritical(JString str)
    {
        throw new NotImplementedException();
    }

    private static void ReleaseStringCritical(JString str)
    {
        throw new NotImplementedException();
    }

    public static T NewWeakGlobalRef<T>(JObject obj) where T : JObject, new()
    {
        var res = Env->Functions->NewWeakGlobalRef(Env, obj.Handle);
        return new T { Handle = res, ReferenceType = ReferenceType.WeakGlobal };
    }

    public static void DeleteWeakGlobalRef(JObject obj)
    {
        Env->Functions->DeleteWeakGlobalRef(Env, obj.Handle);
    }

    public static bool ExceptionCheck()
    {
        return Convert.ToBoolean(Env->Functions->ExceptionCheck(Env));
    }

    private static JObject NewDirectByteBuffer(IntPtr address, int capacity)
    {
        throw new NotImplementedException();
    }

    private static IntPtr GetDirectBufferAddress(JObject buf)
    {
        throw new NotImplementedException();
    }

    private static int GetDirectBufferCapacity(JObject obj)
    {
        throw new NotImplementedException();
    }
}