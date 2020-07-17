using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class GPSManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI locationLabel;
    public bool IsReady = false;
    public Vector2 position;

    private bool devCoords = false;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        print("Starting location service...");
        locationLabel.text = "TEST";

        if(Application.platform == RuntimePlatform.Android)
        {
            // First, check if user has location service enabled
            if (!Input.location.isEnabledByUser)
            {
                locationLabel.text = "No Permission";


                Permission.RequestUserPermission(Permission.FineLocation);


                int maxWaitPermission = 120;
                while (!Input.location.isEnabledByUser && maxWaitPermission > 0)
                {
                    yield return new WaitForSeconds(2);
                    maxWaitPermission--;
                }
            }


            // Start service before querying location
            Input.location.Start();

            // Wait until service initializes
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            // Service didn't initialize in 20 seconds
            if (maxWait < 1)
            {
                print("Timed out");
                yield break;
            }

            // Connection has failed
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                print("Unable to determine device location");
                yield break;
            }
            else
            {
                // Access granted and location value could be retrieved
                print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
                locationLabel.text = Input.location.lastData.latitude + " " + Input.location.lastData.longitude;
                IsReady = true;
            }
        }
        else
        {
            print("test");
            devCoords = true;
            position = new Vector2(4.890748f, 52.372599f);
            IsReady = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!devCoords)
        {
            position = new Vector2(Input.location.lastData.longitude, Input.location.lastData.latitude);
        }
        else
        {
            float speed = 0.0001f;
            if (Input.GetKeyDown("w"))
            {
                position.y += speed;
            }
            else if (Input.GetKeyDown("s"))
            {
                position.y -= speed;
            }
            else if (Input.GetKeyDown("a"))
            {
                position.x -= speed;
            }
            else if (Input.GetKeyDown("d"))
            {
                position.x += speed;
            }
        }
        locationLabel.text = position.x + " " + position.y;

    }
}
