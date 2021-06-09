using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class MathVectorExtensions
{
    public static Vector2Int XZ(this Vector3Int that)
    {
        return new Vector2Int(that.x, that.z);
    }
    public static Vector3Int XZ(this Vector2Int that)
    {
        return new Vector3Int(that.x, 0, that.y);
    }

    public static Vector3 EasingSinIn(this Vector3 that, float t)
    {
        var factor = Mathf.Sin((t * Mathf.PI) * 0.5f);
        return that * factor;
    }
    public static Vector3 EasingSinInBounce(this Vector3 that, float t)
    {
        var factor = Mathf.Sin((t * (Mathf.PI * 2.0f)) * 0.5f);
        return that * factor;
    }
    public static Vector3 EasingSinInCubic(this Vector3 that, float t)
    {
        var factor = t * t * t;
        return that * factor;
    }
    public static Vector3 EasingSinOutCubic(this Vector3 that, float t)
    {
        var factor = 1 - Mathf.Pow(1 - t, 3);
        return that * factor;
    }
    // TODO: this one is bad - to remove
    public static Vector3 EasingSinOut(this Vector3 that, float t) 
    {
        var factor = Mathf.Sin((Mathf.PI * 0.5f) + (t * Mathf.PI) * 0.5f );
        return that * factor;
    }
    public static Vector3 EasingSin(this Vector3 that, float t)
    {
        var factor = Mathf.Sin(t * Mathf.PI);
        return that * factor;
    }
}

