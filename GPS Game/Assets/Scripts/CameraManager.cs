using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private float CameraSpeed = 0.02f;

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDelta = Input.GetTouch(0).deltaPosition;
            transform.transform.Translate(-touchDelta.x * CameraSpeed, -touchDelta.y * CameraSpeed, 0);
        }
    }
}
