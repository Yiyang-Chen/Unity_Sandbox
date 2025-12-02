using System.Collections;
using TMPro;
using UnityEngine;
using Sirenix.OdinInspector;

public class DialogueSentenceWidget : MonoBehaviour
{
    [Title("Name")] [ShowInInspector] [SerializeField]
    protected GameObject nameUI;

    [ShowInInspector] [SerializeField] protected TMP_Text nameText;

    [ReadOnly] [ShowInInspector] [SerializeField]
    protected string dialogueName;

    [Title("Dialogue")] [ShowInInspector] [SerializeField]
    protected TMP_Text text;

    [ReadOnly] [ShowInInspector] [SerializeField]
    protected string showSentence = "";

    protected int tickCounter = 0;
    protected Coroutine tickCo = null;

    public bool isShowingSentence
    {
        get { return tickCo != null; }
    }

    public void Init(string sentence, string name = "")
    {
        showSentence = sentence;
        dialogueName = name;
        ClearDialogue();
        ShowName();
    }

    private void ClearDialogue()
    {
        tickCounter = 0;
        text.text = "";
    }

    private void HideName()
    {
        nameUI.SetActive(false);
        nameText.text = "";
    }

    private void ShowName()
    {
        if (dialogueName == "")
        {
            HideName();
        }
        else
        {
            nameUI.SetActive(true);
            nameText.text = dialogueName;
        }
    }

    public void Show()
    {
        tickCo = StartCoroutine(TypeSentence());
    }

    public void QuickShow()
    {
        if (tickCo != null)
        {
            StopCoroutine(tickCo);
            ClearDialogue();
            text.text = showSentence;
            tickCo = null;
        }
    }

    public void ShowNext(string sentence)
    {
        if (tickCo != null)
        {
            QuickShow();
        }

        Init(sentence, dialogueName);

        Show();
    }

    IEnumerator TypeSentence()
    {
        ClearDialogue();
        char[] chars = showSentence.ToCharArray();
        int showIndex = 0;

        while (showIndex < chars.Length)
        {
            tickCounter++;
            if ((tickCounter % MinimalEnvironment.Instance.GetSystem<DialogueSystem>().TextTick) == 0)
            {
                text.text += chars[showIndex];
                showIndex++;
            }

            yield return null;
        }

        tickCo = null;
    }
}