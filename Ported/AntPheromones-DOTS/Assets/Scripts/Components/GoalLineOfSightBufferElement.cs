using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct GoalLineOfSightBufferElement : IBufferElementData
{
    public bool present;
}