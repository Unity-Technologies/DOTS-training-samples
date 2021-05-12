using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityInput = UnityEngine.Input;

[UpdateInGroup(typeof(Unity.Entities.SimulationSystemGroup))]
public class BulletSpawnerSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        var query = GetEntityQuery(typeof(Tank), typeof(BoardPosition));
        RequireForUpdate(query);

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var times = new double2(Time.ElapsedTime - Time.DeltaTime, Time.ElapsedTime);
        var reloadTime = GetSingleton<ReloadTime>().Value;
        var bulletEntity = GetSingleton<BoardSpawnerData>().BulletPrefab;
        
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();
 
        Entities
            .WithAll<Tank, BoardPosition>()
            .ForEach((
                int entityInQueryIndex,
                ref Translation translation,
                in TargetPosition targetPos,
                in TimeOffset timeOffset) =>
                {
                    var loopCounts = times - timeOffset.Value;
                    loopCounts = math.floor(loopCounts / reloadTime);

                    if (loopCounts.y > loopCounts.x)
                    {
                        ecb.Instantiate(entityInQueryIndex, bulletEntity);
                    }
                }).ScheduleParallel();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}