using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterSentence
{
    public ECharacterSpeaks character = ECharacterSpeaks.Right;
    [TextArea] public string sentence = "";
}

[CreateAssetMenu(menuName = "DialogueSequence/TwoCharacters")]
public class DialogueSequenceTwoCharacters : DialogueSequenceBase
{
    [Title("Background")] public Sprite background;
    [Title("CharacterRight")] public Sprite characterR;
    public string nameR;
    [Title("CharacterLeft")] public Sprite characterL;
    public string nameL;
    [Title("Sentences")] [ShowInInspector] public List<CharacterSentence> sentences = new();
}