using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public enum DoorSide
{
    Left,
    Right,
    Both
}

public class DoorAuthoring : MonoBehaviour
{
    public DoorSide DoorSide;

    class Baker : Baker<DoorAuthoring>
    {
        public override void Bake(DoorAuthoring authoring)
        {
            AddComponent(new Door()
            {
                DoorSide = authoring.DoorSide
            });
        }
    }
}

public struct Door : IComponentData
{
    public DoorSide DoorSide;
    public float elapsedMoveTime;
}

public static class DoorInfo
{
    public static readonly float3 openPos = new float3(0, -0.14f, 0.5f);
    public static readonly float3 closePos = new float3(0, -0.14f, 0.0f);
}