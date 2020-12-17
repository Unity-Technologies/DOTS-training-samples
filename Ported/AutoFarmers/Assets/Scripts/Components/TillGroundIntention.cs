using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct TillGroundIntention : IComponentData 
{
}
public struct TillArea : IComponentData
{
    public RectInt Rect;
}
