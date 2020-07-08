using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple struct to hold the zone ID
/// </summary>
[Serializable]
public struct ZoneID
{
    public int x;
    public int y;

    public ZoneID(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(ZoneID id1, ZoneID id2)
    {
        return id1.Equals(id2);
    }

    public static bool operator !=(ZoneID id1, ZoneID id2)
    {
        return !id1.Equals(id2);
    }
}