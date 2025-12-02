using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSystem : System<DialogueSystem>
{
    DialogueController controller;

    public int SequenceIndex
    {
        get
        {
            if (controller == null)
            {
                return -2;
            }
            else
            {
                return controller.sequenceIndex;
            }
        }
    }

    public bool IsSequencing
    {
        get
        {
            if (currentPanel == null)
            {
                return false;
            }
            else
            {
                return currentPanel.IsSequencing;
            }
        }
    }

    public int TextTick
    {
        get
        {
            if (controller == null)
            {
                return 1;
            }
            else
            {
                return controller.textTick;
            }
        }
    }

    private DialogueSequencePanel currentPanel = null;

    protected override void OnInit()
    {
        controller = GameObject.Find("DialogueController").GetComponent<DialogueController>();
    }

    protected override void OnShutdown()
    {
        controller = null;
        currentPanel = null;
    }

    public void TryNextSequence()
    {
        if (IsSequencing)
        {
            TryFinishCurrentSequence();
        }
        else
        {
            controller.sequenceIndex++;
            if (controller.sequenceIndex < controller.sequence.Count)
            {
                if (currentPanel != null)
                {
                    MinimalEnvironment.Instance.GetSystem<UISystem>().HidePanel(EPanels.SentencePanel);
                }

                BeginCurrentSequence();
            }
        }
    }

    public void BeginCurrentSequence()
    {
        if (controller.sequence[controller.sequenceIndex] is DialogueSequenceSentences sentenceSequence)
        {
            MinimalEnvironment.Instance.GetSystem<UISystem>().ShowPanel<DialogueSentencesPanel>(EPanels.SentencePanel, layer: EUILayer.Top,
                callBack: (panel) =>
                {
                    currentPanel = panel;
                    panel.InitDialogue(sentenceSequence);
                });
        }
    }

    public void TryFinishCurrentSequence()
    {
        currentPanel.TryFinishOrNext();
    }
}