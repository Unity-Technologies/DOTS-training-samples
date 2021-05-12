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
        Entity playerEntity;
        if (!TryGetSingletonEntity<Player>(out playerEntity))
            return;
        
        var times = new double2(Time.ElapsedTime - Time.DeltaTime, Time.ElapsedTime);
        var reloadTime = GetSingleton<ReloadTime>().Value;
        var bulletPrefab = GetSingleton<BoardSpawnerData>().BulletPrefab;
        
        var targetPoint = GetComponent<TargetPosition>(playerEntity);

        const float kDuration = 40f;
        
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
                        var bulletEntity = ecb.Instantiate(entityInQueryIndex, bulletPrefab);

                        ecb.SetComponent(entityInQueryIndex, bulletEntity, new Translation {Value = translation.Value});
                        ecb.SetComponent(entityInQueryIndex, bulletEntity, new TargetPosition {Value = targetPoint.Value});
                        ecb.SetComponent(entityInQueryIndex, bulletEntity, new Time {StartTime = (float)times.y, EndTime = (float)times.y + kDuration});
                        
                        ParabolaUtil.CreateParabolaOverPoint(0.0f, 0.2f, 8.0f, targetPoint.Value.y, out float a, out float b, out float c);
                        ecb.SetComponent(entityInQueryIndex, bulletEntity, new Arc {Value = new float3(a, b, c)});
                    }
                }).ScheduleParallel();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}