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

            if(!zones.ContainsKey(zoneID))
            {
                Vector3 zoneLocation = new Vector3(zoneID.x, -0.05f, zoneID.y);
                GameObject copy = Instantiate(zonePrefab, zoneLocation, Quaternion.identity);
                ZoneData zone = new ZoneData(copy);
                zones.Add(zoneID, zone);
            }
        }
    }

    //Generates the ID based on the game world position of the zone
    private Vector2 GetZoneID(Vector2 coords)
    {
        float x = Mathf.Floor(GPSManager.position.x * scalar);
        float z = Mathf.Ceil(GPSManager.position.y * scalar);

        return new Vector2(x, z);
    }
}
