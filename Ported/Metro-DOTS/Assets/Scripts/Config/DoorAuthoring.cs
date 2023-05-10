using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.VisualScripting;

namespace Metro
{
    public class DoorAuthoring : MonoBehaviour
    {
        public float3 OpenPosition;
        public float3 ClosedPosition;

        private class Baker : Baker<DoorAuthoring>
        {
            public override void Bake(DoorAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Door
                {
                    ClosedPosition = authoring.ClosedPosition,
                    OpenPosition =  authoring.OpenPosition
                });

                AddComponent<UnloadingComponent>(entity);
                AddComponent<DepartingComponent>(entity);
            }
        }
    }

    public struct Door : IComponentData
    {
        public const float OpeningTime = 2.0f;

        public float3 ClosedPosition;
        public float3 OpenPosition;

        public float Timer;
    }
}