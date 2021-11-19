using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class BallisticMover : SystemBase
{
    EndSimulationEntityCommandBufferSystem ecbs;
    BeginSimulationEntityCommandBufferSystem becbs;

    protected override void OnCreate()
    {
        base.OnCreate();
        ecbs = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        becbs = World
            .GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        this.RequireSingletonForUpdate<GlobalData>();
    }



    protected override void OnUpdate()
    {
        var globalDataEntity = GetSingletonEntity<GlobalData>();
        var globalData = GetComponent<GlobalData>(globalDataEntity);

        var time = Time.DeltaTime;
        float3 gravityVector = new float3(0, -2, 0);

        var ecb = ecbs.CreateCommandBuffer();
        var becb = becbs.CreateCommandBuffer();


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
            .WithNone<Decay, InHive>()
            .ForEach((Entity entity, ref Translation translation, in AABB aabb) =>
            {
                var height = translation.Value.y + aabb.center.y - aabb.halfSize.y;
            
            if (height < globalData.BoundsMin.y)
            {
                translation.Value.y = globalData.BoundsMin.y + aabb.halfSize.y - aabb.center.y;
                ecb.RemoveComponent<Ballistic>(entity);
                if (!HasComponent<Food>(entity))
                    ecb.AddComponent(entity, new Decay());
            }
        }).Schedule();

        Entities
        .WithAll<Ballistic, Food>()
        .WithNone<Decay>()
        .ForEach((Entity entity, ref Translation translation, in AABB aabb, in TargetedBy targetedby) =>
        {
            var height = translation.Value.y + aabb.center.y - aabb.halfSize.y;

            if (height < globalData.BoundsMin.y)
            {
                // Despawn the food object
                ecb.DestroyEntity(entity);

                var explosion = becb.Instantiate(globalData.ExplosionPrefab);
                becb.SetComponent<Translation>(explosion, translation);

                for (int i = 0; i < globalData.BeeExplosionCount; ++i)
                {
                    var bee = becb.Instantiate(globalData.BeePrefab);
                    becb.SetComponent<Translation>(bee, translation);
                    BeeSpawner.SetBees(bee, becb, GetComponent<TeamID>(entity));
                }
            }
        }).Schedule();

        ecbs.AddJobHandleForProducer(Dependency);
        becbs.AddJobHandleForProducer(Dependency);
    }
}