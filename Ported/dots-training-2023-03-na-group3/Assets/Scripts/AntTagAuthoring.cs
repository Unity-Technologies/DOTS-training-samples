using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// This tag component is used to identify ants from the rest of entities
/// </summary>
public class AntTagAuthoring : MonoBehaviour
{
}

public struct AntTagComponent : IComponentData
{

}

public class AntTagSpawnerBaker : Baker<AntTagAuthoring>
{
    public override void Bake(AntTagAuthoring authoring)
    {
        AddComponent(new AntTagComponent());
    }
}

