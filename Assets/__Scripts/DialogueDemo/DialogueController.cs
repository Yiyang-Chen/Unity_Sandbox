using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    [Title("DialogueSequence")] public List<DialogueSequenceBase> sequence = new();

    [Title("DialogueParameters")] [MinValue(1)]
    public int textTick = 1;

    [Title("DialogueStates")] [ReadOnly] public int sequenceIndex = -1;

    [ReadOnly]
    [ShowInInspector]
    public bool isSequencing
    {
        get
        {
            if (EditorApplication.isPlaying)
            {
                return MinimalEnvironment.Instance.GetSystem<DialogueSystem>().IsSequencing;
            }
            else
            {
                return false;
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            MinimalEnvironment.Instance.GetSystem<DialogueSystem>().TryNextSequence();
        }
    }
}