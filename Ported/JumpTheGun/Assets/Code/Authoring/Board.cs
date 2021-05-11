
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Board : IComponentData
{
    [Range(1, 1000)]
    public int SizeX;

    [Range(1, 1000)]
    public int SizeY;

    [Range(0.5F, 20F)]
    public float MinHeight;

    [Range(0.5F, 20F)]
    public float MaxHeight;

    [Range(0.1F, 20F)]
    public float ReloadTime;

    [Range(0.1F, 20F)]
    public float Radius;

    [Range(1, 10000)]
    public int NumberOfTanks;

    [Range(0.1F, 5F)]
    public float HitStrength;
    
    public Entity PlaformPrefab;
    public Entity TankPrefab;
}
