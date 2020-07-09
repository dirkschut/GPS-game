using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSManager : MonoBehaviour
{
    private float interval = 1;
    private float totalDelta = 0;
    private int frameCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        totalDelta += Time.deltaTime;
        frameCounter++;

        if(totalDelta >= interval)
        {
            float fps = frameCounter / totalDelta;
            GetComponent<TMPro.TextMeshProUGUI>().text = Mathf.Floor(fps).ToString() + " FPS";
            frameCounter = 0;
            totalDelta = 0f;
        }
        
    }
}
