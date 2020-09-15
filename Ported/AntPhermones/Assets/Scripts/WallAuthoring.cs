using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct WallAuthoring : IComponentData
{
    public Entity wallPrefab;
}
