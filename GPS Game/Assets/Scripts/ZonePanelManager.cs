using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonePanelManager : MonoBehaviour
{
    private ZoneID zoneID;

    // Update is called once per frame
    void Update()
    {
        if(ShouldCastRay())
        {
            Ray camRay = Camera.main.ScreenPointToRay(GetRayPosition());
            RaycastHit raycastHit;
            if (Physics.Raycast(camRay, out raycastHit, 1000f))
            {
                GameObject gameObject = raycastHit.transform.gameObject;
                if (gameObject.CompareTag("Zone"))
                {
                    this.zoneID = gameObject.GetComponent<ZoneIDGetter>().ZoneID;
                    this.GetComponent<TMPro.TextMeshProUGUI>().text = zoneID.x + ", " + zoneID.y;
                }
            }
        }
    }

    private bool ShouldCastRay()
    {
        if(Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began) return true;
        }
        else
        {
            if (Input.GetMouseButtonDown(0)) return true;
        }
        return false;
    }

    private Vector2 GetRayPosition()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            return Input.GetTouch(0).position;
        }
        else
        {
            return Input.mousePosition;
        }
    }
}
