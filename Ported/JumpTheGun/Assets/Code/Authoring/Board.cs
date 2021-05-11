
using System.Collections.Generic;

using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class Board : IComponentData
{
    public int SizeX;
    public int SizeY;

    public float MinHeight;
    public float MaxHeight;

    public int NumberOfTanks;
    public float HitStrength;
    
    public Entity PlaformPrefab;
    public Entity TankPrefab;
}
