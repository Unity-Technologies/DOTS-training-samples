using Unity.Entities;
using Unity.Mathematics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NestAuthoring : MonoBehaviour
{
}
class NestBaker : Baker<NestAuthoring>
{
    public override void Bake(NestAuthoring authoring)
    {
        AddComponent<Velocity>();
    }
}
