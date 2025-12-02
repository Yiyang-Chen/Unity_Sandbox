using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EPlayer : MonoBehaviour
{
    private void OnEnable()
    {
        MinimalEnvironment.Instance.GetSystem<EventSystem>().RemoveEventListener<int>(EGlobalEvent.Example_MonsterDead, MonsterDead);
        MinimalEnvironment.Instance.GetSystem<EventSystem>().AddEventListener<int>(EGlobalEvent.Example_MonsterDead, MonsterDead);
    }

    public void MonsterDead(int mid)
    {
        MinimalEnvironment.Instance.GetSystem<LogSystem>().Debug("Player knows!");
    }
}