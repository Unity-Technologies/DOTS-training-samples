using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class ZombieMovementSystem : SystemBase
{
    const int kChanceToChangeDirection = 3;

    public NativeArray<float2> directions;
    
    protected override void OnCreate()
    {
        directions = new NativeArray<float2>(4, Allocator.Persistent);
        directions[0] = math.left().xy;
        directions[1] = math.right().xy;
        directions[2] = math.up().xy;
        directions[3] = math.down().xy;
    }

    protected override void OnDestroy()
    {
        directions.Dispose();
    }

    protected override void OnUpdate()
    {
        var dirs      = directions;

        Entities.WithReadOnly(dirs).ForEach((ref ZombieTag _, ref Direction direction, ref Random random) => 
        {
            if (direction.MoveState == MoveState.IDLE)
            {
                // some percentage of the time, pick a new direction.
                if (random.Value.NextInt(0, 100) < kChanceToChangeDirection)
                    direction.Value = dirs[random.Value.NextInt(4)];
            }
        })
        .ScheduleParallel();
    }
}
