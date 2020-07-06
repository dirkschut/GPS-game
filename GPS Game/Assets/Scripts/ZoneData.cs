using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ZoneData
{
    private GameObject gameObject;

    public ZoneData(GameObject gameObject, Vector2 imageID)
    {
        this.gameObject = gameObject;
        gameObject.GetComponent<Texturer>().SetTexture(18, (int)imageID.x, (int)imageID.y);
    }
}
