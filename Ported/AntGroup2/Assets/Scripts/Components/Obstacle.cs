using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
struct Obstacle : IComponentData
{
    public float2 position;
}
