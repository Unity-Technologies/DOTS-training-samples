using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct SeatsPerCar : IComponentData
{
    public int rows;
    public int cols;
    public float spacing;
}
