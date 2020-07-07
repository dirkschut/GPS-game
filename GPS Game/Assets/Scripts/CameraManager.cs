using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private float cameraSpeed = 0.001f;
    private float zoomSpeed = 0.1f;

    private float minZoom = 7f;
    private float maxZoom = 150f;

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDelta = Input.GetTouch(0).deltaPosition;
            transform.transform.Translate(-touchDelta.x * cameraSpeed * Camera.main.fieldOfView, -touchDelta.y * cameraSpeed * Camera.main.fieldOfView, 0);
        }
        else if(Input.touchCount == 2)
        {
            Touch firstTouch = Input.GetTouch(0);
            Touch secondTouch = Input.GetTouch(1);
            float touchDistance = Vector2.Distance(firstTouch.position, secondTouch.position);

            Vector2 firstTouchPrevious = firstTouch.position - firstTouch.deltaPosition;
            Vector2 secondTouchPrevious = secondTouch.position - secondTouch.deltaPosition;
            float previousTouchDistance = Vector2.Distance(firstTouchPrevious, secondTouchPrevious);

            float delta = previousTouchDistance - touchDistance;
            Camera.main.fieldOfView += delta * zoomSpeed;
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, minZoom, maxZoom);
        }
    }
}
