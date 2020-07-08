using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class ZoneData
{
    public string texture;
    public ZoneID zoneID;

    private bool hasGameObject = false;

    public ZoneData(GameObject gameObject, ZoneID zoneID)
    {
        //this.gameObject = gameObject;
        this.zoneID = zoneID;
        this.ApplyTexture(gameObject);
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
        if (!hasGameObject)
        {
            GameObject gameObject = CreateGameObject();
            ApplyTexture(gameObject);
            hasGameObject = true;
        }
    }

    /// <summary>
    /// Get the texture and apply it to the zone game object
    /// </summary>
    /// <param name="gameObject"></param>
    public void ApplyTexture(GameObject gameObject)
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

    /// <summary>
    /// Is called if there is no game object in order to create the game object
    /// </summary>
    /// <returns>The zone game object</returns>
    private GameObject CreateGameObject()
    {
        WorldManager worldManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<WorldManager>();
        Vector3 zoneLocation = new Vector3(zoneID.x * WorldManager.zoneSize, -0.05f, -1 * zoneID.y * WorldManager.zoneSize);
        return GameObject.Instantiate(worldManager.zonePrefab, zoneLocation, Quaternion.identity);
    }
}
