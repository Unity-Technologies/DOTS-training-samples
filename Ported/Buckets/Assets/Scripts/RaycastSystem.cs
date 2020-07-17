using System.Diagnostics;
using Fire;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace Water
{
    public class RaycastSystem : SystemBase
    {
        private EntityCommandBufferSystem m_ECBSystem;
        protected override void OnCreate()
        {
            m_ECBSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }


        protected override void OnUpdate()
        {
            var mouseLeftDown = UnityEngine.Input.GetMouseButton(0);
            var mouseRightDown = UnityEngine.Input.GetMouseButton(1);

            if (!mouseLeftDown && !mouseRightDown)
                return;

            var camera = UnityEngine.Camera.main;
            if (camera == null)
                return;

            var ray = camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            new UnityEngine.Plane(UnityEngine.Vector3.up, 0).Raycast(ray, out var enter);
            var hit = (float3)ray.GetPoint(enter);

            var deltaTime = Time.DeltaTime;

            float splashRadius = .5f * 0.5f;
            float splashEffect = 2;
            //var fireBufferEntity = GetSingletonEntity<Fire.FireBuffer>();
            //var fireSystem = GetSingletonEntity<Fire.FireGridSpawnerSystem>();

            // Grab Fire buffer
            //var gridBufferLookup = GetBufferFromEntity<Fire.FireBufferElement>();
            //var gridBuffer = gridBufferLookup[fireBufferEntity];
            //var gridArray = gridBuffer.AsNativeArray();
            var ecbPar = m_ECBSystem.CreateCommandBuffer().ToConcurrent();

            Entities
                .WithNone<ExtinguishAmount>()
                .ForEach((Entity entity, int entityInQueryIndex, ref TemperatureComponent temperature, in Translation trans, in WorldRenderBounds bounds, in LocalToWorld ltw) =>
            {
                //Extinguish Flames
                if (mouseLeftDown)
                {
                    if (math.distancesq(ltw.Position, hit) < splashRadius)
                    {
                        ecbPar.AddComponent(entityInQueryIndex, entity, new ExtinguishAmount { Value = 1f, Propagate = true});
                    }
                }
                else if (mouseRightDown)
                {
                    var topBound = bounds.Value.Center.z + (bounds.Value.Extents.z);
                    var bottomBound = bounds.Value.Center.z - (bounds.Value.Extents.z);
                    var leftBound = bounds.Value.Center.x + (bounds.Value.Extents.x);
                    var rightBound = bounds.Value.Center.x - (bounds.Value.Extents.x);

                    if ((leftBound > hit.x && hit.x > rightBound)
                    && (topBound > hit.z && hit.z > bottomBound))
                    {
                        temperature.Value += 0.5f;
                    }
                }
            }).ScheduleParallel();

        }
    }
}
