using System;
using System.Collections.Generic;
using System.Linq;

public class IOCContainer
{
    private Dictionary<Type, object> mInstances = new Dictionary<Type, object>();

    public void Register<T>(T instance)
    {
        var key = typeof(T);

        if (mInstances.ContainsKey(key))
        {
            mInstances[key] = instance;
        }
        else
        {
            mInstances.Add(key, instance);
        }
    }

    public T Get<T>() where T : class
    {
        var key = typeof(T);

        if (mInstances.TryGetValue(key, out var retInstance))
        {
            return retInstance as T;
        }

        return null;
    }

    public IEnumerable<T> GetInstancesByType<T>()
    {
        var type = typeof(T);
        return mInstances.Values.Where(instance => type.IsInstanceOfType(instance)).Cast<T>();
    }

    public void Clear() => mInstances.Clear();
}