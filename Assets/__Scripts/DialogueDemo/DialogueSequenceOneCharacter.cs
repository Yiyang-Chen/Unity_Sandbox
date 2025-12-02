using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DialogueSequence/OneCharacter")]
public class DialogueSequenceOneCharacter : DialogueSequenceBase
{
    [Title("Background")] public Sprite background;
    [Title("Character")] public Sprite characterR;
    public string nameR;
    [Title("Sentences")] [TextArea] public List<string> sentences = new();
}