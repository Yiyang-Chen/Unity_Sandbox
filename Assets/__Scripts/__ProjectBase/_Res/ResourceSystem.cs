using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manage resource loading
/// </summary>
public class ResourceSystem : System<ResourceSystem>
{
    protected override void OnInit()
    {
    }

    protected override void OnShutdown()
    {
    }
    /// <summary>
    ///     同步加载资源
    ///     Load Synchronizely.
    /// </summary>
    /// <param name="name">
    ///     Name (path) of the object.
    ///     Everything you load should be under the Resource folder.
    /// </param>
    public T Load<T>(string name) where T : Object
    {
        T res = Resources.Load<T>(name);
        if (res is GameObject)
        {
            return GameObject.Instantiate(res);
        }
        else
        {
            return res;
        }
    }

    /// <summary>
    ///     实现异步加载资源协程
    ///     Load Asynchronizely
    /// </summary>
    /// <typeparam name="T">
    ///     Name (path) of the object.
    ///     Everything you load should be under the Resource folder.
    /// </typeparam>
    /// <returns></returns>
    public void LoadAsyn<T>(string name, UnityAction<T> callback) where T : Object
    {
        MinimalEnvironment.Instance.GetSystem<MonoSystem>()?.StartCoroutine(ILoadAsyn<T>(name, callback));
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    private IEnumerator ILoadAsyn<T>(string name, UnityAction<T> callback) where T : Object
    {
        ResourceRequest rr = Resources.LoadAsync<T>(name);
        yield return rr;

        if (rr.asset is GameObject)
        {
            callback(GameObject.Instantiate(rr.asset) as T);
        }
        else
        {
            callback(rr.asset as T);
        }
    }
}