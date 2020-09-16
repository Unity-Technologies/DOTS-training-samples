using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct CityData : IComponentData
{
    public int CountX;
    public int CountZ;
    public int2 height;

}