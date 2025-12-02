using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMonster : MonoBehaviour
{
    public int _id = 101;
    public EPlayer player;
    public EOther[] eOthers;

    void Start()
    {
        Dead();
    }

    void Dead()
    {
        MinimalEnvironment.Instance.GetSystem<LogSystem>().Debug("Monster dead!");
        MinimalEnvironment.Instance.GetSystem<EventSystem>().EventTrigger<int>(EGlobalEvent.Example_MonsterDead, _id);
    }
}