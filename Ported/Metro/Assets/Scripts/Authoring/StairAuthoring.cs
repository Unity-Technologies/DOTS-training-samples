using Unity.Entities;
using UnityEngine;

public class StairAuthoring : MonoBehaviour
{
    public GameObject TopWaypoint;
    public GameObject BottomWaypoint;

    class Baker : Baker<StairAuthoring>
    {
        public override void Bake(StairAuthoring authoring)
        {
            AddComponent(new Stair()
            {
                TopWaypoint = GetEntity(authoring.TopWaypoint),
                BottomWaypoint = GetEntity(authoring.BottomWaypoint)
            });
        }
    }
}

struct Stair : IComponentData
{
    public Entity TopWaypoint;
    public Entity BottomWaypoint;
}
