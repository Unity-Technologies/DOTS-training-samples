using Unity.Entities;
using UnityEngine;

public class WalkwayAuthoring : MonoBehaviour
{
    public Transform LowPoint;
    public Transform HighPoint;
}

class WalkwayBaker : Baker<WalkwayAuthoring>
{
    public override void Bake(WalkwayAuthoring authoring)
    {
        // TODO - Need null check??
        AddComponent(new Walkway
        {
            LowPoint = authoring.LowPoint.position,
            HighPoint = authoring.HighPoint.position
        });
    }
}