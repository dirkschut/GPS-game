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

    [NonSerialized]
    public bool lineActive = false;
    [NonSerialized]
    public UnityEngine.Color lineColor = UnityEngine.Color.green;

    [NonSerialized]
    private GameObject gameObject;


    [NonSerialized]
    private bool gameObjectDestroyed = false;

    [NonSerialized]
    public static readonly int[] intervals = new int[] {
        1,                      //1: 1 hour
        2,                      //2: 2 hours
        4,                      //3: 4 hours
        8,                      //4: 8 hours
        12,                     //5: 12 hours
        24,                     //6: 1 day
        24 * 2,                 //7: 2 days
        24 * 3,                 //8: 3 days
        24 * 7,                 //9: 1 week
        24 * 7 * 2,             //10: 2 weeks
        24 * 30,                //11: 1 month
        24 * 30 * 2,            //12: 2 months
        24 * 30 * 3,            //13: 3 months
        24 * 30 * 4,            //14: 4 months
        24 * 30 * 5,            //15: 5 months
        24 * 30 * 6,            //16: 6 months
        24 * 30 * 7,            //17: 7 months
        24 * 30 * 8,            //18: 8 months
        24 * 30 * 9,            //19: 9 months
        24 * 30 * 10,           //20: 10 months
        24 * 30 * 11,           //21: 11 months
        24 * 30 * 12,           //22: 12 months
    };

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
        tempGameObject.transform.Rotate(new Vector3(-90, 0 , 0));
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
                    lineRenderer.material.color = lineColor;
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

    public bool SetActive(bool active, ZoneID originZOne)
    {
        if (active && (gameObject == null || gameObjectDestroyed))
        {
            gameObjectDestroyed = false;
            gameObject = CreateGameObject();
            reposition(originZOne, WorldManager.zoneSize);
            ApplyTexture();
            ApplyGameObjectText();
            return true;
        }
        else if(gameObject != null && !active)
        {
            GameObject.Destroy(gameObject);
            gameObjectDestroyed = true;
        }
        return false;
    }
}
