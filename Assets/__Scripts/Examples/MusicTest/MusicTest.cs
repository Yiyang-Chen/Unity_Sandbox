using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTest : MonoBehaviour
{
    GUIStyle s;
    GUIStyle s1;

    private AudioSource sound = null;

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 100), "��������"))
        {
            MinimalEnvironment.Instance.GetSystem<MusicSystem>().PlayBkMusic("Remember_Cello_Loop");
        }

        if (GUI.Button(new Rect(0, 100, 100, 100), "��ͣ����"))
            MinimalEnvironment.Instance.GetSystem<MusicSystem>().PauseBkMusic();
        if (GUI.Button(new Rect(0, 200, 100, 100), "ֹͣ����"))
            MinimalEnvironment.Instance.GetSystem<MusicSystem>().StopBkMusic();

        if (GUI.Button(new Rect(0, 300, 100, 100), "������Ч"))
            MinimalEnvironment.Instance.GetSystem<MusicSystem>().PlaySound("Remember_Cello_Loop", false, (s) => { sound = s; });

        if (GUI.Button(new Rect(0, 400, 100, 100), "ֹͣ��Ч"))
        {
            MinimalEnvironment.Instance.GetSystem<MusicSystem>().StopSound(sound);
            sound = null;
        }
    }
}