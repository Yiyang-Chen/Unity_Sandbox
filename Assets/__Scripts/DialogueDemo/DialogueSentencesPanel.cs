using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSentencesPanel : DialogueSequencePanel
{
    public DialogueSentenceWidget sentenceW;
    public Image background;
    [ReadOnly] public List<string> sentences = new List<string>();

    [ReadOnly] [ShowInInspector] [SerializeField]
    private int sentenceIndex = 0;

    [ReadOnly] [ShowInInspector] [SerializeField]
    protected string title = "";

    public override bool IsSequencing
    {
        get { return sentenceIndex < sentences.Count || sentenceW.isShowingSentence; }
    }

    public override void ShowPanel()
    {
        sentenceIndex = 0;
    }

    public override void InitDialogue(DialogueSequenceBase dialogueSequence)
    {
        if (dialogueSequence is DialogueSequenceSentences sentencesSeq)
        {
            background.sprite = sentencesSeq.background;
            sentences = sentencesSeq.sentences;
            title = sentencesSeq.title;
        }

        sentenceW.Init(sentences[0], title);
        sentenceW.Show();
        sentenceIndex++;
    }

    public override void TryFinishOrNext()
    {
        if (sentenceW.isShowingSentence)
        {
            sentenceW.QuickShow();
        }
        else
        {
            if (sentenceIndex < sentences.Count)
            {
                sentenceW.ShowNext(sentences[sentenceIndex]);
                sentenceIndex++;
            }
        }
    }
}