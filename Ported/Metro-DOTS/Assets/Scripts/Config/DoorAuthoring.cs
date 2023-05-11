using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

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
                    Timer = 0f,
                    ClosedPosition = authoring.ClosedPosition,
                    OpenPosition =  authoring.OpenPosition,
                    IsOpening = true
                });
            }
        }
    }

    public struct Door : IComponentData, IEnableableComponent
    {
        public const float OpeningTime = 2.0f;
        
        public float Timer;

        public float3 ClosedPosition;
        public float3 OpenPosition;

        public bool IsOpening;

    }
}