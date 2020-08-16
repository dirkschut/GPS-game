﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSPoint
{
    public readonly Vector2 gpsPosition;
    public readonly DateTime dateTime;
    private GPSPoint next;

    [NonSerialized]
    private GameObject gameObject;

    public GPSPoint(Vector2 gpsPosition, DateTime dateTime, GameObject prefab)
    {
        this.gpsPosition = gpsPosition;
        this.dateTime = dateTime;
        gameObject = GameObject.Instantiate(prefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
    }

    public void SetNext(GPSPoint next)
    {
        this.next = next;
        DrawLine();
    }

    public void Reposition()
    {
        if (gameObject != null)
        {
            Vector2 pos = WorldManager.GPSToGameCoords(gpsPosition);
            gameObject.transform.position = new Vector3(pos.x, 0.5f, pos.y);
            if(next != null)
            {
                DrawLine();
            }
        }
    }

    private void DrawLine()
    {
        Debug.LogWarning("DRAWLINE");
        LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, GetGameobjectPosition());
        lineRenderer.SetPosition(1, next.GetGameobjectPosition());
    }

    public Vector3 GetGameobjectPosition()
    {
        return gameObject.transform.position;
    }
}
