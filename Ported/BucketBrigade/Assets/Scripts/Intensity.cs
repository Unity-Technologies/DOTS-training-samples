using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Intensity : IComponentData
{
    public float TestValue; // should things be called Value or not?
}
