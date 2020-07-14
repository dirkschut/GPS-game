using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class WorldManager : MonoBehaviour
{
    public GPSManager GPSManager;
    public Camera camera;
    public GameObject player;
    public GameObject zonePrefab;
    public TMPro.TextMeshProUGUI scoreLabel;
    public TMPro.TextMeshProUGUI zonesLabel;

    private ZoneID playerZone;

    public const int scalar = 10000;
    public const int zoomLevel = 18;
    public const int zoneSize = 10;

    private bool forceOnEnter = false;
    private bool centerCameraOnPlayer = true;

    private Dictionary<ZoneID, ZoneData> zones = new Dictionary<ZoneID, ZoneData>();

    // Start is called before the first frame update
    void Start()
    {
        LoadWorld();
        forceOnEnter = true;
    }

    // Update is called once per frame
    void Update()
    {
        foreach(ZoneData zoneData in zones.Values)
        {
            zoneData.Update(player.transform.position);
        }

        if (GPSManager.IsReady)
        {
            ZoneID zoneID = GetZoneID(GPSManager.position);

            if (zoneID != playerZone || forceOnEnter)
            {
                playerZone = zoneID;

                if (!PosHasZone(GPSManager.position))
                {
                    ZoneData zone = new ZoneData(zoneID);
                    zone.SetActive(true);
                    zones.Add(zoneID, zone);
                    forceOnEnter = true;
                }

                EnterZone(zoneID, forceOnEnter);

                if (forceOnEnter)
                {
                    RepositionPlayer(zoneID);
                    CenterCameraOnPlayer(player.transform.position);
                }

                forceOnEnter = false;
            }
            else
            {
                RepositionPlayer(zoneID);
            }
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

    public int CalculateScore()
    {
        int score = 0;

        foreach(ZoneData zoneData in zones.Values)
        {
            score += zoneData.points;
        }

        return score;
    }

    /// <summary>
    /// Gets called every time a player enters a zone and at game launch.
    /// </summary>
    /// <param name="zoneID">THe zoneID of zone the player entered.</param>
    private void EnterZone(ZoneID zoneID, bool forceReposition)
    {
        zones[zoneID].OnEnter();
        scoreLabel.text = "Score: " + CalculateScore();
        zonesLabel.text = "Zones: " + zones.Count;

        bool forceRecalc = false;
        int lineCount = 0;
        int maxLines = 5;
        int zoneCount = 0;
        int maxZones = 250;
        foreach (KeyValuePair<ZoneID, ZoneData> zone in zones.OrderBy(key => key.Value.GetDistanceFromPlayer(playerZone)))
        {
            if(zoneCount < maxZones || lineCount < maxLines)
            {
                zoneCount++;
                if (zone.Value.SetActive(true))
                {
                    forceRecalc = true;
                }

                if (zone.Value.CanVisit() && lineCount < maxLines)
                {
                    zone.Value.lineActive = true;
                    lineCount++;
                }
                else
                {
                    zone.Value.lineActive = false;
                }
            }
            else
            {
                zone.Value.SetActive(false);
            }
        }

        int recalcAMount = 1000;
        if (player.transform.position.x >= recalcAMount || player.transform.position.z >= recalcAMount || forceReposition || forceRecalc)
        {
            RepositionZones();
        }

        SaveWorld();
    }

    /// <summary>
    /// Repositions the zones around the position of the player.
    /// </summary>
    private void RepositionZones()
    {
        Debug.Log("Reposition");
        foreach (ZoneData zoneData in zones.Values)
        {
            zoneData.reposition(playerZone, zoneSize);
        }
    }

    private void CenterCameraOnPlayer(Vector3 playerPos)
    {
        Vector3 camPos = camera.transform.position;
        camPos.x = playerPos.x;
        camPos.z = playerPos.z;
        camera.transform.position = camPos;
    }

    private void RepositionPlayer(ZoneID zoneID)
    {
        //Interpolate position in zone and move player to said position.
        Vector2 gpsLocation = GPSManager.position;
        Vector2 thisZoneStart = new Vector2(tilex2long(zoneID.x, zoomLevel), tiley2lat(zoneID.y, zoomLevel));
        Vector2 nextZoneStartX = new Vector2(tilex2long(zoneID.x + 1, zoomLevel), tiley2lat(zoneID.y, zoomLevel));
        Vector2 nextZoneStartY = new Vector2(tilex2long(zoneID.x, zoomLevel), tiley2lat(zoneID.y + 1, zoomLevel));
        float percentageX = (gpsLocation.x - thisZoneStart.x) / (nextZoneStartX.x - thisZoneStart.x);
        float percentageY = (gpsLocation.y - thisZoneStart.y) / (nextZoneStartY.y - thisZoneStart.y);
        if (percentageY < 0) percentageY = 0;
        if (percentageX < 0) percentageX = 0;

        Vector3 playerPos = player.transform.position;
        playerPos.x = zones[zoneID].GetGameObject().transform.position.x + percentageX * zoneSize - 0.5f * zoneSize;
        playerPos.z = zones[zoneID].GetGameObject().transform.position.z + -1 * percentageY * zoneSize + 0.5f * zoneSize;
        player.transform.position = playerPos;

        if (centerCameraOnPlayer)
        {
            CenterCameraOnPlayer(playerPos);
        }
    }

    public void OnCenterButtonClick()
    {
        centerCameraOnPlayer = !centerCameraOnPlayer;
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
