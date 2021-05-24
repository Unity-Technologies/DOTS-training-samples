using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Animation;
using Unity.Transforms;
using Unity.Jobs;

namespace AutoFarmers
{   
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class PlantSimulationSystemGroup : ComponentSystemGroup
    {

    }
    [UpdateInGroup(typeof(PlantSimulationSystemGroup))]
    public class SoldPlantSimulationSystem : SystemBase
    {
        EntityCommandBufferSystem commandBufferSystem;
        protected override void OnCreate()
        {
            commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        protected override void OnUpdate()
        {
            var parallelCommandBuffer = commandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            var deltaTime = Time.DeltaTime;
            var smooth = 1f - math.pow(0.1f, deltaTime);
            var farm = GetSingleton<Farm>();
            Entities.ForEach((Entity entity, int entityInQueryIndex, ref Plant.Sold sold, ref Translation translation, ref NonUniformScale nonUniformScale, in Plant plant) =>
            {
                ref var t = ref sold.ElapsedTime;
                t += deltaTime;
                if (t >= 1f)
                {
                    parallelCommandBuffer.DestroyEntity(entityInQueryIndex, entity);
                }
                else
                {
                    var y = AnimationCurveEvaluator.Evaluate(t, ref farm.SoldPlantYCurve);
                    var x = plant.Position.x + 0.5f;
                    var z = plant.Position.y + 0.5f;
                    var scaleXZ = AnimationCurveEvaluator.Evaluate(t, ref farm.SoldPlantXZScaleCurve);
                    var scaleY = AnimationCurveEvaluator.Evaluate(t, ref farm.SoldPlantYScaleCurve);
                    var position = new float3(x, y, z);
                    translation.Value += (position - translation.Value) * smooth * 3f;
                    nonUniformScale.Value = new float3(scaleXZ, scaleY, scaleXZ);
                }
            }).ScheduleParallel();
        }
    }

    [UpdateInGroup(typeof(PlantSimulationSystemGroup))]
    [UpdateAfter(typeof(SoldPlantSimulationSystem))]
    public class ApplyPlantGrowthScaleSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTimeDiv10 = Time.DeltaTime/10f;
            Entities
                .ForEach((Entity entity,ref NonUniformScale nonUniformScale, ref Plant.Growth growth) =>
                {
                    growth.Value = math.min(growth.Value + deltaTimeDiv10, 1f);
                    var t = math.sqrt(growth.Value);
                    var xz = math.smoothstep(0, 1, t * t * t * t * t) * 0.9f + 0.1f;
                    var scale = new float3(xz, t, xz);
                    nonUniformScale.Value = math.select(scale, nonUniformScale.Value * scale, HasComponent<Plant.Sold>(entity));
                }).ScheduleParallel();
            //var soldJob = Entities
            //    .WithAll<Plant.Sold>()
            //    .ForEach((ref NonUniformScale nonUniformScale, ref Plant.Growth growth) =>
            //{
            //    growth.Value = math.min(growth.Value + deltaTimeDiv10, 1f);
            //    var t = math.sqrt(growth.Value);
            //    nonUniformScale.Value.y *= t;
            //    nonUniformScale.Value.xz *= math.smoothstep(0, 1, t * t * t * t * t) * 0.9f + 0.1f;
            //}).ScheduleParallel(Dependency);
            //var notSoldJob = Entities
            //    .WithNone<Plant.Sold>()
            //    .ForEach((ref NonUniformScale nonUniformScale, ref Plant.Growth growth) =>
            //    {
            //        growth.Value = math.min(growth.Value + deltaTimeDiv10, 1f);
            //        var t = math.sqrt(growth.Value);
            //        nonUniformScale.Value.y = t;
            //        nonUniformScale.Value.xz = math.smoothstep(0, 1, t * t * t * t * t) * 0.9f + 0.1f;
            //    }).ScheduleParallel(Dependency);
            //Dependency = JobHandle.CombineDependencies(soldJob, notSoldJob);
        }
    }

}