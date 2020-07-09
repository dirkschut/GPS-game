using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraManager : MonoBehaviour
{
    private float touchCameraSpeed = 0.001f;
    private float mouseCameraSpeed = 0.0005f;
    private float touchZoomSpeed = 0.1f;
    private float mouseZoomSpeed = 5f;

    private float minZoom = 7f;
    private float maxZoom = 170f;

    private Vector2 oldMousePos;

    // Update is called once per frame
    void Update()
    {
        if(Application.platform == RuntimePlatform.Android)
        {
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Vector2 touchDelta = Input.GetTouch(0).deltaPosition;
                transform.transform.Translate(-touchDelta.x * touchCameraSpeed * Camera.main.fieldOfView, -touchDelta.y * touchCameraSpeed * Camera.main.fieldOfView, 0);
            }
            //Zoom
            else if (Input.touchCount == 2)
            {
                Touch firstTouch = Input.GetTouch(0);
                Touch secondTouch = Input.GetTouch(1);
                float touchDistance = Vector2.Distance(firstTouch.position, secondTouch.position);

                Vector2 firstTouchPrevious = firstTouch.position - firstTouch.deltaPosition;
                Vector2 secondTouchPrevious = secondTouch.position - secondTouch.deltaPosition;
                float previousTouchDistance = Vector2.Distance(firstTouchPrevious, secondTouchPrevious);

                float delta = previousTouchDistance - touchDistance;
                Camera.main.fieldOfView += delta * touchZoomSpeed;
            }
        }
        else
        {
            //Pan
            if (Input.GetMouseButtonDown(0))
            {
                oldMousePos = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0) && oldMousePos != null)
            {
                Vector2 currentMousePos = Input.mousePosition;
                Vector2 deltaPos = oldMousePos -= currentMousePos;
                oldMousePos = currentMousePos;
                Camera.main.transform.Translate(deltaPos * Camera.main.fieldOfView * mouseCameraSpeed);
            }

            //Zoom
            float scrollDelta = Input.mouseScrollDelta.y;
            Camera.main.fieldOfView -= scrollDelta * mouseZoomSpeed;
        }

        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, minZoom, maxZoom);

    }
}
