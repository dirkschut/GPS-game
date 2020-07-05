using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class GPSManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI locationLabel;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        print("Starting location service...");
        locationLabel.text = "TEST";

        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            locationLabel.text = "No Permission";

            #if PLATFORM_ANDROID
            Permission.RequestUserPermission(Permission.FineLocation);
            #endif

            yield break;
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
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        locationLabel.text = Input.location.lastData.latitude + " " + Input.location.lastData.longitude;
    }
}
