using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class Resource : IComponentData
{
    [Range(3f, 10000f)]
    public float Capacity = 100;
}
