using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class WorldManager : MonoBehaviour
{
    public GPSManager GPSManager;
    public Camera camera;
    public GameObject player;
    public GameObject zonePrefab;

    public const int scalar = 10000;
    public const int zoomLevel = 18;
    public const int zoneSize = 10;

    private ZoneID previousPosition;
    private bool firstEntry = true;

    private Dictionary<ZoneID, ZoneData> zones = new Dictionary<ZoneID, ZoneData>();

    // Start is called before the first frame update
    void Start()
    {
        LoadWorld();
    }

    // Update is called once per frame
    void Update()
    {
        if (GPSManager.IsReady)
        {
            ZoneID zoneID = GetZoneID(GPSManager.position);

            if (zoneID != previousPosition)
            {
                Vector3 camPos = camera.transform.position;
                camPos.x = zoneID.x * zoneSize;
                camPos.z = -1 * zoneID.y * zoneSize;
                camera.transform.position = camPos;

                Vector3 playerPos = player.transform.position;
                playerPos.x = zoneID.x * zoneSize;
                playerPos.z = -1 * zoneID.y * zoneSize;
                player.transform.position = playerPos;

                if (!PosHasZone(GPSManager.position))
                {
                    Vector3 zoneLocation = new Vector3(zoneID.x * zoneSize, -0.05f, -1 * zoneID.y * zoneSize);
                    GameObject copy = Instantiate(zonePrefab, zoneLocation, Quaternion.identity);
                    ZoneData zone = new ZoneData(copy, zoneID);
                    zones.Add(zoneID, zone);
                    SaveWorld();
                }

                zones[zoneID].OnEnter();
            }
            previousPosition = zoneID;
        }
    }

    /// <summary>
    /// Saves the world data.
    /// </summary>
    private void SaveWorld()
    {
        print("saving world");
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, zones);
        file.Close();
    }

    private void LoadWorld()
    {
        if(File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            print("Loading world");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            zones = (Dictionary<ZoneID, ZoneData>)bf.Deserialize(file);
            file.Close();
            foreach (ZoneData zoneData in zones.Values)
            {
                zoneData.InitializeFromSave();
            }
        }
    }

    /// <summary>
    /// Generates the image ID based on the given GPS coords
    /// </summary>
    /// <param name="coords">GPS coords</param>
    /// <returns>Image ID</returns>
    private ZoneID GetZoneID(Vector2 coords)
    {
        int x = long2tilex(coords.x, zoomLevel);
        int y = lat2tiley(coords.y, zoomLevel);
        return new ZoneID(x, y);
    }

    /// <summary>
    /// Checks to see if given GPS coords have a zone.
    /// </summary>
    /// <param name="coords">GPS coords</param>
    /// <returns>true=has a zone</returns>
    bool PosHasZone(Vector2 coords)
    {
        ZoneID imageID = GetZoneID(coords);
        if (zones.ContainsKey(imageID))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Saves the texture into a zonedata object to be saved.
    /// </summary>
    /// <param name="zoneID">The zone in which the texture has to be saved.</param>
    /// <param name="texture">The texture to be saved.</param>
    public void SaveTexture(ZoneID zoneID, Texture2D texture)
    {
        if (zones.ContainsKey(zoneID))
        {
            zones[zoneID].SaveTexture(texture, zoneID);
        }
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
