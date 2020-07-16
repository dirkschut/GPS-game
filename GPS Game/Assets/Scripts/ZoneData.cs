using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

[Serializable]
public class ZoneData
{
    public string texture;
    public ZoneID zoneID;
    public DateTime lastVisit;
    public int points;
    public DateTime nextVisit;
    public bool lineActive = false;

    [NonSerialized]
    private GameObject gameObject;

    [NonSerialized]
    public static readonly int[] intervals = new int[] { 1, 2, 4, 8, 12, 24, 48, 72, 168, 336, 504, 672};

    public ZoneData(ZoneID zoneID)
    {
        this.zoneID = zoneID;
    }

    /// <summary>
    /// Saves the texture in the zone data to be saved.
    /// </summary>
    /// <param name="texture">The texture</param>
    public void SaveTexture(Texture2D texture, ZoneID zoneID)
    {
        if(! Directory.Exists(Application.persistentDataPath + "/zones/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/zones/");
        }

        byte[] bytes = texture.EncodeToPNG();
        string path = GetImagePath();
        System.IO.File.WriteAllBytes(path, bytes);
        this.texture = path;
        Debug.Log("Saved " + path);
    }

    /// <summary>
    /// Returns the path to the image in the save.
    /// </summary>
    /// <returns></returns>
    public string GetImagePath()
    {
        return Application.persistentDataPath + "/zones/" + zoneID.x + "_" + zoneID.y + ".png";
    }

    /// <summary>
    /// Is called when the player enters the zone
    /// </summary>
    public void OnEnter()
    {
        if (gameObject == null)
        {
            gameObject = CreateGameObject();
            ApplyTexture();
        }

        if(DateTime.Now >= nextVisit || lastVisit == null)
        {
            points++;
            lastVisit = DateTime.Now;
            int newInterval = intervals[intervals.Length - 1];
            if(points <= intervals.Length)
            {
                newInterval = intervals[points - 1];
            }
            nextVisit = lastVisit.AddHours(newInterval);
            ApplyGameObjectText();
        }
    }

    /// <summary>
    /// Get the texture and apply it to the zone game object
    /// </summary>
    /// <param name="gameObject"></param>
    public void ApplyTexture()
    {
        Debug.Log("Loading Image");
        if (File.Exists(GetImagePath()))
        {
            gameObject.GetComponent<Texturer>().SetTextureFromFile(GetImagePath());
        }
        else
        {
            gameObject.GetComponent<Texturer>().SetTextureFromWeb(18, zoneID);
        }
    }

    internal GameObject GetGameObject()
    {
        return gameObject;
    }

    /// <summary>
    /// Is called if there is no game object in order to create the game object
    /// </summary>
    /// <returns>The zone game object</returns>
    private GameObject CreateGameObject()
    {
        WorldManager worldManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<WorldManager>();
        Vector3 zoneLocation = new Vector3(zoneID.x * WorldManager.zoneSize, -0.05f, -1 * zoneID.y * WorldManager.zoneSize);
        GameObject tempGameObject = GameObject.Instantiate(worldManager.zonePrefab, zoneLocation, Quaternion.identity);
        tempGameObject.GetComponent<ZoneIDGetter>().ZoneID = zoneID;
        return tempGameObject;
    }

    private void ApplyGameObjectText()
    {
        TextMeshPro tmpNextVisit = gameObject.transform.Find("next").GetComponent<TextMeshPro>();

        string nextVisitString;
        if (nextVisit.Date == DateTime.Today)
        {
            nextVisitString = nextVisit.ToString("H:mm:ss");
        }
        else
        {
            nextVisitString = nextVisit.ToString("dd MMM");
        }
        tmpNextVisit.text = nextVisitString;

        TextMeshPro tmpPoints = gameObject.transform.Find("points").GetComponent<TextMeshPro>();
        tmpPoints.text = points.ToString();
    }

    public void Update(Vector3 playerPosition)
    {
        if(gameObject != null)
        {
            ApplyGameObjectText();
            LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();


            if (DateTime.Now > nextVisit)
            {
                gameObject.GetComponent<Renderer>().material.color = UnityEngine.Color.green;
                if (lineActive)
                {
                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPosition(0, playerPosition + new Vector3(0, 0.5f, 0));
                    lineRenderer.SetPosition(1, gameObject.transform.position);
                }
                else
                {
                    lineRenderer.positionCount = 0;
                }

            }
            else if (DateTime.Today == nextVisit.Date)
            {
                gameObject.GetComponent<Renderer>().material.color = UnityEngine.Color.yellow;
                lineRenderer.positionCount = 0;
            }
            else
            {
                gameObject.GetComponent<Renderer>().material.color = UnityEngine.Color.white;
                lineRenderer.positionCount = 0;
            }
        }
        
    }

    /// <summary>
    /// Repositions the zone relative to the player.
    /// </summary>
    /// <param name="playerZone">The zoneID of the zone the player occupies.</param>
    /// <param name="zoneSize">The size of each zone.</param>
    public void reposition(ZoneID playerZone, float zoneSize)
    {
        if(gameObject != null)
        {
            int dx = zoneID.x - playerZone.x;
            int dy = zoneID.y - playerZone.y;

            Vector3 zoneLocation = new Vector3(dx * zoneSize, -0.05f, -1 * dy * zoneSize);
            gameObject.transform.position = zoneLocation;
        }
    }

    public float GetDistanceFromPlayer(ZoneID playerZone)
    {
        return Vector2.Distance(new Vector2(zoneID.x, zoneID.y), new Vector2(playerZone.x, playerZone.y));
    }

    public bool CanVisit()
    {
        if(DateTime.Now >= nextVisit || lastVisit == null)
        {
            return true;
        }
        return false;
    }

    public bool SetActive(bool active)
    {
        if (active && gameObject == null)
        {
            gameObject = CreateGameObject();
            ApplyTexture();
            ApplyGameObjectText();
            return true;
        }
        else if(gameObject != null && !active)
        {
            GameObject.Destroy(gameObject);
        }
        return false;
    }
}
