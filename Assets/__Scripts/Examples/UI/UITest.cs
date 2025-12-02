using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MinimalEnvironment.Instance.GetSystem<UISystem>().ShowPanel<TestPanel>(EPanels.ExamplePanel, ShowPanelOver, EUILayer.Mid);
    }

    private void ShowPanelOver(TestPanel panel)
    {
        panel.InitialFinished();
    }


    // Update is called once per frame
    void Update()
    {
    }
}