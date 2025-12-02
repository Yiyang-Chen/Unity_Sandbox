using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRaycastTest : MonoBehaviour
{
    public bool mainCam = false;

    public bool ignore = false;

    // Update is called once per frame
    void Update()
    {
        if (mainCam)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //___Singletons.Log.Debug("mainCam: hit " + hit.transform.name);
            }
        }
    }
}