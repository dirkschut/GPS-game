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

    public const int scalar = 10000;
    public const int zoomLevel = 18;
    public const int zoneSize = 10;

    private Vector2 previousPosition;

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
            Vector2 imageID = GetImageID(GPSManager.position);

            if(imageID != previousPosition)
            {
                Vector3 camPos = camera.transform.position;
                camPos.x = imageID.x * zoneSize;
                camPos.z = -1 * imageID.y * zoneSize;
                camera.transform.position = camPos;

                Vector3 playerPos = player.transform.position;
                playerPos.x = imageID.x * zoneSize;
                playerPos.z = -1 * imageID.y * zoneSize;
                player.transform.position = playerPos;
            }

            if(!PosHasZone(GPSManager.position))
            {
                Vector3 zoneLocation = new Vector3(imageID.x * zoneSize, -0.05f, -1 * imageID.y * zoneSize);
                GameObject copy = Instantiate(zonePrefab, zoneLocation, Quaternion.identity);
                ZoneData zone = new ZoneData(copy, imageID);
                zones.Add(imageID, zone);
            }
            previousPosition = imageID;
        }
    }

    /// <summary>
    /// Generates the zone ID from the given GPS coords
    /// </summary>
    /// <param name="coords">GPS coords</param>
    /// <returns>Zone ID</returns>
    private Vector2 GetZoneID(Vector2 coords)
    {
        float x = Mathf.Floor(coords.x * scalar);
        float z = Mathf.Ceil(coords.y * scalar);

        return new Vector2(x, z);
    }

    /// <summary>
    /// Generates the image ID based on the given GPS coords
    /// </summary>
    /// <param name="coords">GPS coords</param>
    /// <returns>Image ID</returns>
    private Vector2 GetImageID(Vector2 coords)
    {
        int x = long2tilex(coords.x, zoomLevel);
        int y = lat2tiley(coords.y, zoomLevel);
        return new Vector2(x, y);
    }

    /// <summary>
    /// Checks to see if given GPS coords have a zone.
    /// </summary>
    /// <param name="coords">GPS coords</param>
    /// <returns>true=has a zone</returns>
    bool PosHasZone(Vector2 coords)
    {
        Vector2 imageID = GetImageID(coords);
        if (zones.ContainsKey(imageID))
        {
            return true;
        }
        return false;
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
