using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

// Board setup singleton
// create/remove to run InitBoardSetup OnUpdate once
[GenerateAuthoringComponent]
public struct BoardSetup : IComponentData
{
    public Entity cell0Prefab;
    public Entity cell1Prefab;
    public Entity wallPrefab;
}