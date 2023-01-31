using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class StairAuthoring : MonoBehaviour
{
    public GameObject TopWaypoint;
    public GameObject TopWalkwayWaypoint;
    public GameObject BottomWaypoint;

    class Baker : Baker<StairAuthoring>
    {
        public override void Bake(StairAuthoring authoring)
        {
            AddComponent(new Stair()
            {
                TopWaypoint = authoring.TopWaypoint.transform.position,
                TopWalkwayWaypoint = authoring.TopWalkwayWaypoint.transform.position,
                BottomWaypoint = authoring.BottomWaypoint.transform.position
            });
        }
    }
}

public struct Stair : IComponentData
{
    public float3 TopWaypoint;
    public float3 TopWalkwayWaypoint;
    public float3 BottomWaypoint;
}
