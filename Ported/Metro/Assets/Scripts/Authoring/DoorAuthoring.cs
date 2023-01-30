using Unity.Entities;
using UnityEngine;

public enum DoorSide
{
    Left,
    Right
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

struct Door : IComponentData
{
    public DoorSide DoorSide;
}
