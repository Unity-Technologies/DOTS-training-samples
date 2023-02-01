using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

struct Human : IComponentData
{
    public int TrainID;
    public float3 currentDestination;
    public Entity WagonOfChoice;
    public float Height;
    public Color HumanColor;
}