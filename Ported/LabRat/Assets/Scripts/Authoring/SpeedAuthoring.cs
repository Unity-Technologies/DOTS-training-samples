using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

struct Speed : IComponentData
{
    public float Value;
}
