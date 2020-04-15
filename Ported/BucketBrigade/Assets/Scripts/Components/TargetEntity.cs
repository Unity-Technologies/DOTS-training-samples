using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
[GenerateAuthoringComponent]
public struct TargetEntity : IComponentData
{
    public Entity target;
}
