
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(Unity.Entities.SimulationSystemGroup))]
public class PlatformCollision : SystemBase
{
    protected override void OnCreate()
    {
        var query = GetEntityQuery(typeof(Bullet), typeof(BoardTarget));
        RequireForUpdate(query);
    }

    protected override void OnUpdate()
    {
        if (TryGetSingleton<IsPaused>(out _))
            return;

        float currentTime = (float)Time.ElapsedTime;

        var hitCount = new NativeList<int2>(GetEntityQuery(typeof(Bullet)).CalculateEntityCount(), Allocator.TempJob);
        var hitCountWriter = hitCount.AsParallelWriter();

        Entities
            .WithAll<Bullet>()
            .ForEach((int entityInQueryIndex, Entity entity, in BoardTarget target, in Time time) =>
            {
                if (time.EndTime <= currentTime)
                    hitCountWriter.AddNoResize(target.Value);

            }).ScheduleParallel();

        Entities
            .WithDisposeOnCompletion(hitCount)
            .WithAll<Platform, BoardPosition>()
            .WithReadOnly(hitCount)
            .ForEach((int entityInQueryIndex, Entity entity, ref WasHit hit, in BoardPosition boardPosition) => {

                for (int index = 0; index < hitCount.Length; index++)
                {
                    if (hitCount[index].Equals(boardPosition.Value))
                        hit.Count++;
                }

            }).ScheduleParallel();
    }
}
