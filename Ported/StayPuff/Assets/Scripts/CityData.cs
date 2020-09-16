using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct CityData : IComponentData
{
    public int CountX;
    public int CountZ;
    //public Vector3 BoundsMax;
    //public Vector3 BoundsMin;
}