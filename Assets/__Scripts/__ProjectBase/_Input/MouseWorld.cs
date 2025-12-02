using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseWorld : MonoBehaviour
{
    [SerializeField]
    private LayerMask debugLayerMask;
    public static Vector3 NoHitPoint = new Vector3(-10000, -10000, -10000);
    public bool showDebug = true;
    
    // Update is called once per frame
    private void Update()
    {
        // 在UI上就不处理鼠标事件了
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
    
        if (showDebug)
        {
            LayerMask layerMask;
            switch (MinimalEnvironment.Instance.GetSystem<GameStateSystem>().GetGameState())
            {
                default:
                    layerMask = debugLayerMask;
                    break;
            }

            if (layerMask != -1)
            {
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(mouseRay, out RaycastHit hit, float.MaxValue, layerMask))
                {
                    transform.position = hit.point;
                }
            }
            
        }
        
        //TODO: input system
        if (Input.GetMouseButtonDown(0))
        {
            //按顺序判断鼠标点击效果，true表示处理完毕，屏蔽后续处理直接返回
            switch (MinimalEnvironment.Instance.GetSystem<GameStateSystem>().GetGameState())
            {
                default:
                    break;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            switch (MinimalEnvironment.Instance.GetSystem<GameStateSystem>().GetGameState())
            {
                default:
                    break;
            }
        }
    }

    public static Vector3 GetMouseGridPlanePosition(LayerMask layerMask)
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseRay, out RaycastHit hit, float.MaxValue, layerMask))
        {
            return hit.point;
        }
        return NoHitPoint;
    }
    
}
