using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public struct AntMovement : IComponentData
{
    public float speed;
    public float3 pos;
}

public class AntMovementAuthoring : MonoBehaviour
{
    public float speed;
    public float3 pos;
}

public class AntMovementBaker : Baker<AntMovementAuthoring>
{
    public override void Bake(AntMovementAuthoring authoring)
    {
        AddComponent(new AntMovement
        {
            speed = authoring.speed, pos = authoring.pos
        });
    }
}