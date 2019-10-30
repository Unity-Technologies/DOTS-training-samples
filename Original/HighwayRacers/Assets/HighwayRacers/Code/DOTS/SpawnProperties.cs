using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[Serializable]
public struct CarSpawnProperties : ISharedComponentData
{
    public Entity carPrefab;
    public float defaultSpeedMin;
    public float defaultSpeedMax;
    public float overtakeSpeedMin;
    public float overtakeSpeedMax;
    public Color defaultColor;
    public Color slowSpeedColor;
    public Color highSpeedColor;
    public float mergeSpaceMin;
    public float mergeSpaceMax;
    public float mergeLeftDistanceMin;
    public float mergeLeftDistanceMax;
    public float acceleration;
    public float brakeDeceleration;
    public float laneSwitchSpeed;
    public float maxOvertakeTime;
}

