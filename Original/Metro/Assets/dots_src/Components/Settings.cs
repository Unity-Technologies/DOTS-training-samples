using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Settings : IComponentData
{
    /// <summary>
    /// Max train speed in m/s
    /// </summary>
    public float MaxSpeed;
    
    public float CarriageSizeWithMargins;
    
    /// <summary>
    /// How long time in seconds do we wait at station?
    /// </summary>
    public float TimeAtStation;

    /// <summary>
    /// How far from the station does the train start breaking (in meters)
    /// </summary>
    public float BreakingDistance;
}
