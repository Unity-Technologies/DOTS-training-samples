using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// Encapsulates the pheromone value for a cell.
/// We divide our whole environment into resolution*resolution cells. each cell has a pheromone value.
/// PheromoneSystem maintains the Pheromone values in cells.
/// </summary>
public class PheromoneDataAuthoring : MonoBehaviour
{
}

public struct PheromoneData : IBufferElementData
{
    public float value;
}

public class PheromoneDataBaker : Baker<PheromoneDataAuthoring>
{
    public override void Bake(PheromoneDataAuthoring authoring)
    {
        AddBuffer<PheromoneData>();
    }
}
