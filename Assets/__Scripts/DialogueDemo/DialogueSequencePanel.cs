using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DialogueSequencePanel : BasePanel
{
    public virtual bool IsSequencing
    {
        get { return true; }
    }

    public virtual void InitDialogue(DialogueSequenceBase dialogueSequence)
    {
    }

    public virtual void TryFinishOrNext()
    {
    }
}