namespace JNISharp.NativeInterface;

public record JClass : JObject
{
    private Dictionary<Tuple<string, string>, JFieldID> FieldCache { get; } = new ();

    private Dictionary<Tuple<string, string>, JMethodID> MethodCache { get; } = new ();

    public JFieldID GetFieldID(string name, string sig)
    {
        Tuple<string, string> key = new (name, sig);

        if (FieldCache.TryGetValue(key, out var found))
        {
            return found;
        }

        var id = JNI.GetFieldID(this, name, sig);
        FieldCache.Add(key, id);
        return id;
    }

    public JFieldID GetStaticFieldID(string name, string sig)
    {
        Tuple<string, string> key = new (name, sig);

        if (FieldCache.TryGetValue(key, out var found))
        {
            return found;
        }

        var id = JNI.GetStaticFieldID(this, name, sig);
        FieldCache.Add(key, id);
        return id;
    }

    public JMethodID GetMethodID(string name, string sig)
    {
        Tuple<string, string> key = new (name, sig);

        if (MethodCache.TryGetValue(key, out var found))
        {
            return found;
        }

        var id = JNI.GetMethodID(this, name, sig);
        MethodCache.Add(key, id);
        return id;
    }

    public JMethodID GetStaticMethodID(string name, string sig)
    {
        Tuple<string, string> key = new (name, sig);

        if (MethodCache.TryGetValue(key, out var found))
        {
            return found;
        }

        var id = JNI.GetStaticMethodID(this, name, sig);
        MethodCache.Add(key, id);
        return id;
    }

    public T GetStaticObjectField<T>(string name, string sig) where T : JObject, new()
    {
        return JNI.GetStaticObjectField<T>(this, GetStaticFieldID(name, sig));
    }

    public T GetStaticField<T>(string name)
    {
        return JNI.GetStaticField<T>(this, GetStaticFieldID(name, JNI.GetTypeSignature<T>()));
    }

    public void SetStaticField<T>(string name, T value)
    {
        JNI.SetStaticField(this, GetStaticFieldID(name, JNI.GetTypeSignature<T>()), value);
    }

    public T GetObjectField<T>(JObject obj, string name, string sig) where T : JObject, new()
    {
        return JNI.GetObjectField<T>(obj, GetFieldID(name, sig));
    }

    public T GetField<T>(JObject obj, string name)
    {
        return JNI.GetField<T>(obj, GetFieldID(name, JNI.GetTypeSignature<T>()));
    }

    public void SetObjectField(JObject obj, string name, string sig, JObject value)
    {
        JNI.SetObjectField(obj, GetFieldID(name, sig), value);
    }

    public void SetField<T>(JObject obj, string name, T value)
    {
        JNI.SetField(obj, GetFieldID(name, JNI.GetTypeSignature<T>()), value);
    }

    public T CallStaticObjectMethod<T>(string name, string sig, params JValue[] args) where T : JObject, new()
    {
        return JNI.CallStaticObjectMethod<T>(this, GetStaticMethodID(name, sig), args);
    }

    public T CallStaticMethod<T>(string name, string sig, params JValue[] args)
    {
        return JNI.CallStaticMethod<T>(this, GetStaticMethodID(name, sig), args);
    }

    public void CallStaticVoidMethod(string name, string sig, params JValue[] args)
    {
        JNI.CallStaticVoidMethod(this, GetMethodID(name, sig), args);
    }

    public T CallObjectMethod<T>(JObject obj, string name, string sig, params JValue[] args) where T : JObject, new()
    {
        return JNI.CallObjectMethod<T>(obj, GetMethodID(name, sig), args);
    }

    public T CallMethod<T>(JObject obj, string name, string sig, params JValue[] args)
    {
        return JNI.CallMethod<T>(obj, GetMethodID(name, sig), args);
    }

    public void CallVoidMethod(JObject obj, string name, string sig, params JValue[] args)
    {
        JNI.CallVoidMethod(obj, GetMethodID(name, sig), args);
    }

    public T NewObject<T>(string name, string sig, params JValue[] args) where T : JObject, new()
    {
        return JNI.NewObject<T>(this, GetMethodID(name, sig), args);
    }
}