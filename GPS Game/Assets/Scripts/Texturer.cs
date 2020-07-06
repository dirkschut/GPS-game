using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Texturer : MonoBehaviour
{
    public void SetTexture(int zoom, int x, int y)
    {
        string url = "http://tile.openstreetmap.org/" + zoom + "/" + x + "/" + y + ".png";
        StartCoroutine(DownloadImage(url));

    }

    IEnumerator DownloadImage(string MediaUrl)
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

            GetComponent<Renderer>().material.mainTexture = texture;
        }
            
    }
}
