using System.Diagnostics;
using Fire;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Water
{
    public class RaycastSystem : SystemBase
    {
        //private EndSimulationEntityCommandBufferSystem ecbSystem;

        //protected override void OnCreate()
        //{
        //ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        //}

        protected override void OnUpdate()
        {
            //var ecb = ecbSystem.CreateCommandBuffer().ToConcurrent();
            //EntityManager manager = Unity.Entities.World.DefaultGameObjectInjectionWorld.EntityManager;
            var camera = UnityEngine.Camera.main;
            if (camera == null)
                return;

            var ray = camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            new UnityEngine.Plane(UnityEngine.Vector3.up, 0).Raycast(ray, out var enter);
            var hit = (float3) ray.GetPoint(enter);

            var deltaTime = Time.DeltaTime;
            var mouseDown = UnityEngine.Input.GetMouseButton(0);

            var fireBufferEntity = GetSingletonEntity<Fire.FireBuffer>();
            //var fireSystem = GetSingletonEntity<Fire.FireGridSpawnerSystem>();
            
            // Grab Fire buffer
            var gridBufferLookup = GetBufferFromEntity<Fire.FireBufferElement>();
            var gridBuffer = gridBufferLookup[fireBufferEntity];
            var gridArray = gridBuffer.AsNativeArray();

            Entities.ForEach((int entityInQueryIndex, ref Entity entity, ref TemperatureComponent temperature, ref Color color, in LocalToWorld ltw) =>
            {
                var dist = mouseDown ? math.distancesq(ltw.Position, hit) / 10 : 1;

                if (dist < .4)
                {
                    //ecb.SetComponent(entityInQueryIndex, gridArray[entityInQueryIndex].FireEntity, new TemperatureComponent { Value = 1 });
                    temperature = new TemperatureComponent{Value = 1 };
                   // SetComponent(entity, new TemperatureComponent { Value = 1 });
                    var clamped = math.clamp(1 - dist, 0, 1);
                    color.Value = new float4(1, 0, 0, 1);
                }



                //var dist = mouseDown ? math.distancesq(ltw.Position, hit) / 10 : 1;

                //if(dist < .5)
                //{
                //    var clamped = math.clamp(1 - dist, 0, 1);
                //    color.Value = new float4(1,0,0,1);
                //}

                //ecb.AddComponent(entityInQueryIndex, entity, new BucketActiveTag { });
            }).ScheduleParallel();

            //ecbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
