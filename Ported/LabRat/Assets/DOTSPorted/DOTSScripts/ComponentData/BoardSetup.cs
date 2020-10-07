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
    public Entity cellPrefab;
    public Entity wallPrefab;
    public float checkValue;
}