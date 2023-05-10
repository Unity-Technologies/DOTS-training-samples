using UnityEngine;
using Unity.Entities;

namespace Metro
{
    public class DoorAuthoring : MonoBehaviour
    {
        public GameObject LeftDoor;
        public GameObject RightDoor;

        private class Baker : Baker<DoorAuthoring>
        {
            public override void Bake(DoorAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Door
                {
                    LeftDoor = GetEntity(authoring.LeftDoor, TransformUsageFlags.Dynamic),
                    RightDoor = GetEntity(authoring.RightDoor, TransformUsageFlags.Dynamic)
                });
            }
        }
    }

    public struct Door : IComponentData
    {
        public Entity LeftDoor;
        public Entity RightDoor;

        public const float DoorWidth = 0.3f;
        public const float OpeningTime = 2.0f;
    }
}