using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GPSPoint
{
    public readonly float lon;
    public readonly float lat;
    public readonly DateTime dateTime;
    public float actualDistance;

    [NonSerialized]
    private GameObject gameObject;

    [NonSerialized]
    private GPSPoint nextPoint;

    public GPSPoint(Vector2 gpsPosition, DateTime dateTime)
    {
        lon = gpsPosition.x;
        lat = gpsPosition.y;
        this.dateTime = dateTime;
        CreateGameObject();
    }

    public void Reposition()
    {
        if (gameObject != null)
        {
            Vector2 pos = WorldManager.GPSToGameCoords(new Vector2(lon, lat));
            gameObject.transform.position = new Vector3(pos.x, 0.5f, pos.y);
            if(nextPoint != null)
            {
                DrawLine();
            }
        }
    }

    public void DrawLine()
    {
        if(gameObject != null && nextPoint != null)
        {
            LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, GetGameobjectPosition());
            lineRenderer.SetPosition(1, nextPoint.GetGameobjectPosition());
        }
    }

    public Vector3 GetGameobjectPosition()
    {
        return gameObject.transform.position;
    }

    public Vector2 GetGPSPosition()
    {
        return new Vector2(lon, lat);
    }

    public void CreateGameObject()
    {
        WorldManager worldManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<WorldManager>();
        gameObject = GameObject.Instantiate(worldManager.pointPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        Reposition();
    }

    public void SetNext(GPSPoint nextPoint)
    {
        this.nextPoint = nextPoint;
        DrawLine();
    }

    public void DestroyGameObject()
    {
        GameObject.Destroy(gameObject);
        gameObject = null;
    }

    public bool HasGameObject()
    {
        if(gameObject != null)
        {
            return true;
        }
        return false;
    }
}
