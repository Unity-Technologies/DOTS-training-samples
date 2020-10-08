using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(MoveTowardsTargetSystem))]
public class StartNextCommandSystem : SystemBase
{
    private EntityQuery bucketQuery;
    private EntityQuery waterQuery;

    protected override void OnCreate()
    {
        EntityQueryDesc queryDesc = new EntityQueryDesc
        {
            All = new [] {ComponentType.ReadOnly<BucketTag>(), ComponentType.ReadOnly<Pos>()},
            None = new [] {ComponentType.ReadOnly<BucketOwner>()}
        };
        bucketQuery = GetEntityQuery(queryDesc);

        waterQuery = GetEntityQuery(ComponentType.ReadOnly<WaterTag>(), ComponentType.ReadOnly<Pos>());
    }

    protected override void OnUpdate()
    {
        Random rand = new Random((uint)System.DateTime.UtcNow.Ticks);

        // Make both of these Pos instead of Translation
        NativeArray<Pos> bucketPositions = bucketQuery.ToComponentDataArrayAsync<Pos>(Allocator.TempJob, out JobHandle j1);
        NativeArray<Entity> bucketEntities = bucketQuery.ToEntityArrayAsync(Allocator.TempJob, out JobHandle j2);
        NativeArray<Pos> waterPositions = waterQuery.ToComponentDataArrayAsync<Pos>(Allocator.TempJob, out JobHandle j3);
        Dependency = JobHandle.CombineDependencies(Dependency, JobHandle.CombineDependencies(j1, j2, j3));

        Entities
            .WithName("StartNextCommand")
            .WithDisposeOnCompletion(bucketPositions)
            .WithDisposeOnCompletion(bucketEntities)
            .WithDisposeOnCompletion(waterPositions)
            .ForEach((ref ExecutingCommand executingCommand, ref CurrentBotCommand currentCommand, ref Target target, ref DynamicBuffer<CommandBufferElement> commandQueue,
                in Pos pos, in HasBucket hasBucket) =>
            {
                if (currentCommand.Command != Command.None)
                    return;
                if (commandQueue.Length == 0)
                    return;

                currentCommand.Command = commandQueue[0].Value;
                commandQueue.RemoveAt(0);
                executingCommand.Value = true;

                target.ReachedTarget = false;
                switch (currentCommand.Command)
                {
                    case Command.Move:
                        target.Position = GetRandomPosition(ref rand);
                        break;
                    case Command.FindOrKeepBucket:
                        if (hasBucket.Has)
                        {
                            target.Position = pos.Value;
                            target.Entity = hasBucket.Entity;
                        }
                        else
                        {
                            target.Position = GetNearestPos(pos.Value, bucketPositions, out int nearestIndex);
                            target.Entity = bucketEntities[nearestIndex];
                        }
                        break;
                    case Command.FillBucket:
                        target.Position = GetNearestPos(pos.Value, waterPositions, out _);
                        break;
                    case Command.EmptyBucket:
                        target.Position = GetRandomPosition(ref rand);
                        break;
                }
            }).ScheduleParallel();
    }

    // This is just a placeholder, will eventually become GetNearestFire, GetNearestBucket, GetChainPosition etc.
    static float2 GetRandomPosition(ref Random rand)
    {
        float radius = 10;
        float2 pos = rand.NextFloat2() * radius - new float2(radius * 0.5f) + new float2(10f, 10f);
        return pos;
    }

    static float2 GetNearestPos(in float2 pos, in NativeArray<Pos> positions, out int nearestIndex)
    {
        nearestIndex = 0;
        float nearestDist = float.MaxValue;
        float2 nearestPos = pos;

        for (int i = 0; i < positions.Length; i++)
        {
            float dist = math.distance(pos, positions[i].Value);
            if (dist < nearestDist)
            {
                nearestIndex = i;
                nearestDist = dist;
                nearestPos = positions[i].Value;
            }
        }

        return nearestPos;
    }

    static float2 GetNearestFire(ref Random rand)
    {
        return GetRandomPosition(ref rand);
    }
}