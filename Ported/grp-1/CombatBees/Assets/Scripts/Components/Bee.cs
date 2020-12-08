using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public struct Bee : IComponentData
{
    public float teamID;
    public Entity currentTargetEntity;
    public Entity baseEntity;
    public Entity centerEntity;
    public Translation currentTargetTransform;
    public Translation baseTargetTransform;
    public Translation CenterTargetTransform;
}

public struct BeeTeam1 : IComponentData
{
}

public struct BeeTeam2 : IComponentData
{
}
