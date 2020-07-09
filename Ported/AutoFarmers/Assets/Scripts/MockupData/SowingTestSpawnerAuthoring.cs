using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct SowingTestSpawner : IComponentData
{
    public Entity Farmer;
    public int NumFarmers;
}
