using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DialogueSequence/SentencesOnly")]
public class DialogueSequenceSentences : DialogueSequenceBase
{
    [Title("Background")] public Sprite background;
    [Title("title")] public string title = "";
    [Title("Sentences")] [TextArea] public List<string> sentences = new();
}