using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //单例测试
        //NewMono.GetInstance().Test();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MinimalEnvironment.Instance.GetSystem<PoolSystem>().GetObj("_Prefabs/Examples/Cube",
                (obj) => { obj.transform.localScale = Vector3.one * 2; });
        }

        if (Input.GetMouseButtonDown(1))
        {
            MinimalEnvironment.Instance.GetSystem<PoolSystem>().GetObj("_Prefabs/Examples/Sphere",
                (obj) => { obj.transform.localScale = Vector3.one * 2; });
            //ResourceMgr.GetInstance().Load<GameObject>("Test/Sphere");
        }
    }
}