using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Handles assigning the texture to the zone
/// </summary>
public class Texturer : MonoBehaviour
{
    /// <summary>
    /// Entrypoint to get the texture from the web.
    /// </summary>
    /// <param name="zoom">The zoom leven (normally 18)</param>
    /// <param name="zoneID">The zoneID to get the texture for</param>
    public void SetTextureFromWeb(int zoom, ZoneID zoneID)
    {
        string url = "http://tile.openstreetmap.org/" + zoom + "/" + zoneID.x + "/" + zoneID.y + ".png";
        StartCoroutine(DownloadImage(url, zoneID));
    }

    /// <summary>
    /// The actual texture downloader
    /// </summary>
    /// <param name="MediaUrl">The URL to download the image from</param>
    /// <param name="zoneID">The zone ID to apply the texture to</param>
    /// <returns></returns>
    IEnumerator DownloadImage(string MediaUrl, ZoneID zoneID)
    {
        print("Downloading Image: " + MediaUrl);
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            print(request.error);
        else
        {
            print("Success....");
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Color[] pix = texture.GetPixels(0, 0, 256, 256);
            System.Array.Reverse(pix, 0, pix.Length);
            texture.SetPixels(0, 0, 256, 256, pix);
            texture.Apply();
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<WorldManager>().SaveTexture(zoneID, texture);

            GetComponent<Renderer>().material.mainTexture = texture;
        }

    }

    /// <summary>
    /// Assign a texture to the zone from a file
    /// </summary>
    /// <param name="path">The path to the texture</param>
    public void SetTextureFromFile(string path)
    {
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(256, 256);
        texture.LoadImage(fileData);
        GetComponent<Renderer>().material.mainTexture = texture;
    }
}
