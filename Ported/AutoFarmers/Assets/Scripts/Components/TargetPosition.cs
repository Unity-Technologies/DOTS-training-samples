using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct TargetPosition :  IComponentData
{
   public float3 Target;
}
