using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class BallisticMover : SystemBase
{
    EndSimulationEntityCommandBufferSystem ecbs;

    protected override void OnCreate()
    {
        base.OnCreate();
        ecbs = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var globalDataEntity = GetSingletonEntity<GlobalData>();
        var globalData = GetComponent<GlobalData>(globalDataEntity);

        var time = Time.DeltaTime;
        float3 gravityVector = new float3(0, -2, 0);

        var ecb = ecbs.CreateCommandBuffer();

        Entities
            .WithAll<Ballistic>()
            .ForEach((ref Velocity velocity) => { velocity.Value += gravityVector * time; }).Schedule();

        Entities
            .WithAll<Ballistic>()
            .ForEach((ref Translation translation, in Velocity velocity) =>
            {
                translation.Value = translation.Value + velocity.Value * time;
            }).Schedule();

        Entities
            .WithAll<Ballistic>()
            .WithNone<Decay>()
            .ForEach((Entity entity, ref Translation translation, ref Velocity velocity, in AABB aabb) =>
            {
                var abspos = math.abs(translation.Value + aabb.center)+aabb.halfSize;
                if (abspos.x > globalData.BoundsMax.x)
                {
                    if (velocity.Value.x * translation.Value.x > 0.0f)
                    {
                        velocity.Value.x *= -0.5f;
                    }
                }
                
                if (abspos.z > globalData.BoundsMax.z)
                {
                    if (velocity.Value.z * translation.Value.z > 0.0f)
                    {
                        velocity.Value.z *= -0.5f;
                    }
                }
                
                var height = translation.Value.y + aabb.center.y - aabb.halfSize.y;

                if (height < globalData.BoundsMin.y)
                {
                    translation.Value.y = globalData.BoundsMin.y + aabb.halfSize.y - aabb.center.y;
                    ecb.RemoveComponent<Ballistic>(entity);
                    if (HasComponent<Food>(entity))
                    {
                        // if food is in hive, spawn new bees
                    }
                    else
                        ecb.AddComponent(entity, new Decay());
                }
            }).Schedule();

        ecbs.AddJobHandleForProducer(Dependency);
    }
}