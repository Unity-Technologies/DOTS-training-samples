using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct LineOfSightBufferElement : IBufferElementData
{
    public bool present;
}