using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

[GenerateAuthoringComponent]
public struct CarSpawner : IComponentData
{
    public Entity CarPrefab;
    public float3 carScaleV1;
    public float3 carScaleV2;
    public float3 carScaleV3;
}
