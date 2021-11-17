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
        var time = Time.DeltaTime;
        float3 gravityVector = new float3(0, -2, 0);

        var ecb = ecbs.CreateCommandBuffer();
        
        Entities
            .WithAll<Ballistic>()
            .ForEach((ref Velocity velocity) =>
        {
            velocity.Value += gravityVector * time;
        }).Schedule();

        Entities
            .WithAll<Ballistic>()
            .ForEach((ref Translation translation, in Velocity velocity) => 
        {
             translation.Value = translation.Value + velocity.Value * time;
        }).Schedule();

        Entities
            .WithAll<Ballistic>()
            .WithNone<Decay>()
            .ForEach((Entity entity, ref Translation translation) =>
        {
            if (translation.Value.y < -5)
            {
                translation.Value.y = -5;
                ecb.RemoveComponent<Ballistic>(entity);
                if (HasComponent<Food>(entity))
                {
                    // if food is in hive, spawn new bees
                }
                else
                    ecb.AddComponent(entity, new Decay {Rate = 1.0f});
            }
        }).Schedule();
        
        ecbs.AddJobHandleForProducer(Dependency);
    }
}
