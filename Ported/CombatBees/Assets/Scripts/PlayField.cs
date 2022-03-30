using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class PlayField
{
    public static readonly Vector3 size = new Vector3(100f, 20f, 30f);
    public static readonly float goalDepth = size.x * (.5f * (1f - .8f));
    public static readonly Vector3 origin = Vector3.zero;
    public static readonly float gravity = -20f;
    public static readonly float resourceHeight = 1.5f;
}