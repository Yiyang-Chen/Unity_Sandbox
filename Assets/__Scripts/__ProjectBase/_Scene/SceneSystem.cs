using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换管理
/// Load scene manager.
/// </summary>
public class SceneSystem : System<SceneSystem>
{
    protected override void OnInit()
    {
    }

    protected override void OnShutdown()
    {
    }
    /// <summary>
    /// 同步切换
    /// Load scene Synchronizely.
    /// </summary>
    public void LoadScene(string name, UnityAction callback)
    {
        SceneManager.LoadScene(name);
        callback();
    }

    /// <summary>
    /// 异步切换
    /// Load scene Asynchronizely.
    /// </summary>
    public void LoadSceneAsyn(string name, UnityAction callback = null)
    {
        MinimalEnvironment.Instance.GetSystem<MonoSystem>()?.StartCoroutine(ILoadSceneAsyn(name, callback));
    }

    private IEnumerator ILoadSceneAsyn(string name, UnityAction callback)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        // 可以通过ao.process获取场景加载进度
        // You can use ao.process to get the process of loading a scene.
        while (ao.isDone)
        {
            // 可以在这里更新进度条
            // You can update the process bar here.
            MinimalEnvironment.Instance.GetSystem<EventSystem>()?.EventTrigger(EGlobalEvent.Example_UpdateProgressBar, ao.progress);
            yield return ao.progress;
        }

        yield return ao;

        if (callback != null) callback();
    }
}