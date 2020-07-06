using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public GPSManager GPSManager;
    public Camera camera;
    public GameObject player;
    public GameObject zonePrefab;

    private int scalar = 1000;

    private Dictionary<Vector2, ZoneData> zones = new Dictionary<Vector2, ZoneData>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GPSManager.IsReady)
        {
            Vector3 camPos = camera.transform.position;
            camPos.x = GPSManager.position.x * scalar;
            camPos.z = GPSManager.position.y * scalar;
            camera.transform.position = camPos;

            Vector3 playerPos = player.transform.position;
            playerPos.x = GPSManager.position.x * scalar;
            playerPos.z = GPSManager.position.y * scalar;
            player.transform.position = playerPos;

            Vector2 zoneID = GetZoneID(GPSManager.position);
            Vector2 imageID = GetImageID(GPSManager.position);

            if(!zones.ContainsKey(zoneID))
            {
                Vector3 zoneLocation = new Vector3(zoneID.x, -0.05f, zoneID.y);
                GameObject copy = Instantiate(zonePrefab, zoneLocation, Quaternion.identity);
                ZoneData zone = new ZoneData(copy, imageID);
                zones.Add(zoneID, zone);
            }
        }
    }

    //Generates the ID based on the game world position of the zone
    private Vector2 GetZoneID(Vector2 coords)
    {
        float x = Mathf.Floor(coords.x * scalar);
        float z = Mathf.Ceil(coords.y * scalar);

        return new Vector2(x, z);
    }

    private Vector2 GetImageID(Vector2 coords)
    {
        int x = long2tilex(coords.y, 18);
        int y = lat2tiley(coords.x, 18);
        return new Vector2(x, y);
    }

    int long2tilex(float lon, int z)
    {
        return (int)Mathf.Floor((lon + 180.0f) / 360.0f * (1 << z));
    }

    int lat2tiley(float lat, int z)
    {
        return (int)Mathf.Floor((1 - Mathf.Log(Mathf.Tan(Mathf.Deg2Rad * lat) + 1 / Mathf.Cos(Mathf.Deg2Rad * lat)) / Mathf.PI) / 2 * (1 << z));
    }

    float tilex2long(int x, int z)
    {
        return x / (float)(1 << z) * 360.0f - 180;
    }

    float tiley2lat(int y, int z)
    {
        float n = Mathf.PI - 2.0f * Mathf.PI * y / (float)(1 << z);
        return 180.0f / Mathf.PI * Mathf.Atan(0.5f * (Mathf.Exp(n) - Mathf.Exp(-n)));
    }
}
