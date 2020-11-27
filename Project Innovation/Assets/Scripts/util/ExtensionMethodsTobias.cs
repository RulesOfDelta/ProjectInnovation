﻿using UnityEditor;
using UnityEngine;

public static class ExtensionMethodsTobias
{
    public static Vector3 XZ(this Vector2 v)
    {
        return new Vector3(v.x, 0, v.y);
    }

    public static Vector3 XY(this Vector2 v)
    {
        return v;
    }

    public static Vector3 YZ(this Vector2 v)
    {
        return new Vector3(0, v.x, v.y);
    }

    public static bool Contains(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }

    public static FMOD.Studio.EventInstance CreateSound(this string path)
    {
        return FMODUnity.RuntimeManager.CreateInstance(path);
    }

    public static void PlayAtPos(this FMOD.Studio.EventInstance instance, Vector3 pos)
    {
        instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(pos));
        instance.start();
    }
}