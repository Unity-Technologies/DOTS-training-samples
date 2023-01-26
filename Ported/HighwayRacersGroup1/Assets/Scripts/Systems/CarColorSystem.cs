using Aspects;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace Systems
{
    [BurstCompile]
    partial struct CarColorSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Creating an EntityCommandBuffer to defer the structural changes required by instantiation.
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var car in SystemAPI.Query<CarAspect>())
            {
                state.EntityManager.SetComponentData(car.CarEntity, CreateColorBySpeed(car.Speed));
            }
        }

        private URPMaterialPropertyBaseColor CreateColorBySpeed(float speed)
        {
            var hue = ((int)(speed/10) * 0.2f) % 1f;
            var color = UnityEngine.Color.HSVToRGB(hue, 1.0f, 1.0f);
            return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
        }
    }
}