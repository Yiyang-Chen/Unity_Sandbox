using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MinimalEnvironment.Instance.GetSystem<EventSystem>().RemoveEventListener(EGlobalEvent.Example_JumpPressed, CheckInputDown);
        MinimalEnvironment.Instance.GetSystem<EventSystem>().AddEventListener(EGlobalEvent.Example_JumpPressed, CheckInputDown);
    }

    // Update is called once per frame
    private void CheckInputDown()
    {
        MinimalEnvironment.Instance.GetSystem<LogSystem>().Debug("jump down");
    }
}