using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct ObstacleBufferElement : IBufferElementData
{
    public bool present;
}