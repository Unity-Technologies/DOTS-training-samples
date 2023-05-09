using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace Metro
{
    public class PassengerAuthoring : MonoBehaviour
    {
        class Baker : Baker<PassengerAuthoring>
        {
            public override void Bake(PassengerAuthoring authoring)
            {
                //var random = new Random(2345);
                var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale);
                AddComponent(entity, new PassengerComponent
                {
                    Height = 1f
                });
            }
        }
    }

    public struct PassengerComponent : IComponentData
    {
        public float Height;
        public Entity Head;
    }
}
