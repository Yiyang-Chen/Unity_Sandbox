using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//The class can save the objects under a father object named as the path of the object in the resource folder.
public class PoolData
{
    public GameObject fatherObj;
    private HashSet<GameObject> objectSet;
    public List<GameObject> poolList;

    public PoolData(GameObject obj, GameObject father)
    {
        fatherObj = new GameObject(obj.name);
        fatherObj.transform.SetParent(father.transform);

        poolList = new List<GameObject>();
        objectSet = new HashSet<GameObject>();
        PushObj(obj);
    }
    
    public bool Contains(GameObject obj)
    {
        return objectSet.Contains(obj);
    }

    public void PushObj(GameObject obj)
    {
        poolList.Add(obj);
        objectSet.Add(obj);

        obj.transform.SetParent(fatherObj.transform);
        obj.SetActive(false);
    }

    public GameObject GetObj()
    {
        GameObject obj0 = null;

        obj0 = poolList[0];
        poolList.RemoveAt(0);
        objectSet.Remove(obj0);

        obj0.transform.SetParent(null);
        obj0.SetActive(true);

        return obj0;
    }
}

//Object pool
//Don't save everything in the pool. If the game is very big, it may cause memory problem.
//You can GC manually as you wish.
public class PoolSystem : System<PoolSystem>
{
    //Container of all the pools
    public Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();
    private GameObject pool;

    protected override void OnInit()
    {
    }

    /// <summary>
    /// Get the object you want.
    /// !!!IMPORTANT: This function loads asynchronizely if no enough objects in the pool. Be sure to deal with asyn issues.
    /// </summary>
    /// <param name="name">
    /// Name (path) of the object.
    /// Everything you load should be under the Resource folder.
    /// </param>
    /// <param name="callback">What do you want the object to do after SetActive.</param>
    public void GetObj(string name, UnityAction<GameObject> callback)
    {
        if (poolDic.ContainsKey(name) && poolDic[name].poolList.Count > 0)
        {
            callback(poolDic[name].GetObj());
        }
        else
        {
            MinimalEnvironment.Instance.GetSystem<ResourceSystem>().LoadAsyn<GameObject>(name, (obj) =>
            {
                if (obj == null)
                {
                    MinimalEnvironment.Instance.GetSystem<LogSystem>().Error($"[PoolMgr] LoadAsyn {name} failed");
                    callback(obj);
                    return;
                }
                obj.name = name;
                callback(obj);
            });
        }
    }

    /// <summary>
    /// Push the object into pool synchronizely.
    /// </summary>
    /// <param name="name">
    /// Name (path) of the object.
    /// Everything you load should be under the Resource folder.
    /// </param>
    /// <param name="obj">That obj you want to push.</param>
    public Coroutine PushObj(
        string name, 
        GameObject obj, 
        float delayPushTime = -1f,
        UnityAction<GameObject> callback = null)
    {
        if (pool == null)
        {
            pool = new GameObject("Pool");
        }

        if (delayPushTime > 0)
        {
            return MinimalEnvironment.Instance.GetSystem<MonoSystem>().StartCoroutine(DelayPushObj(name, obj, delayPushTime, callback));
        }
        else
        {
            PushObjectImmediately(name, obj);
            callback?.Invoke(obj);
            return null;
        }
    }

    public void CancelDelayPush(Coroutine coroutine)
    {
        MinimalEnvironment.Instance.GetSystem<MonoSystem>().StopCoroutine(coroutine);
    }

    private IEnumerator DelayPushObj(
        string name, 
        GameObject obj, 
        float maxExistTime,
        UnityAction<GameObject> callback)
    {
        yield return new WaitForSeconds(maxExistTime);
        PushObjectImmediately(name, obj);
        callback?.Invoke(obj);
    }

    private void PushObjectImmediately(string name, GameObject obj)
    {
        if (poolDic.ContainsKey(name))
        {
            var poolData = poolDic[name];
            if (poolData.Contains(obj))
            {
                MinimalEnvironment.Instance.GetSystem<LogSystem>().Warn($"[PoolMgr] 重复放入池 {name} {obj.GetInstanceID()}");
                obj.SetActive(false);
                return;
            }
            poolData.PushObj(obj);
        }
        else
        {
            poolDic.Add(name, new PoolData(obj, pool));
        }
    }
    
    //GC manually.
    //You can add more GC functions as you wish.
    public void Clear()
    {
        poolDic.Clear();
        pool = null;
    }

    protected override void OnShutdown()
    {
        Clear();
    }
}