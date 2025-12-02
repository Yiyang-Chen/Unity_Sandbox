using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 外部类mono添加事件的方法
/// </summary>
public class Testtest
{
    public void Update()
    {
        MinimalEnvironment.Instance.GetSystem<LogSystem>().Debug("Test");
    }

    public IEnumerator Test123()
    {
        yield return new WaitForSeconds(1f);
        MinimalEnvironment.Instance.GetSystem<LogSystem>().Debug("123");
    }
}

public class MonoTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Testtest t = new Testtest();
        MinimalEnvironment.Instance.GetSystem<MonoSystem>().StartCoroutine(t.Test123());
    }
}