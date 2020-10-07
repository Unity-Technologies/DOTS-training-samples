using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct BoardInfo : IComponentData
{
    public int height;
    public int width;
    public int minNumberOfHoles;
    public int maxNumberOfHoles;
    public int numberOfWalls;
}
