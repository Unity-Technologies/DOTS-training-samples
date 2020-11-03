using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MovementSystem : SystemBase
{
    public NativeArray<float2> directions;
    
    protected override void OnCreate()
    {
        directions = new NativeArray<float2>(4, Allocator.Persistent);
        directions[0] = math.left().xy;
        directions[1] = math.right().xy;
        directions[2] = math.up().xy;
        directions[3] = math.down().xy;

        var e = EntityManager.CreateEntity(typeof(ZombieTag), typeof(Position), typeof(Random));
#if UNITY_EDITOR
        EntityManager.SetName(e, "Zombie");
#endif
        EntityManager.SetComponentData(e, new Position());
        EntityManager.SetComponentData(e, new Random(1234));
    }

    protected override void OnDestroy()
    {
        directions.Dispose();
    }

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        float speed     = 1;
        var   dirs      = directions;

        Entities.ForEach((ref ZombieTag _, ref Position position, ref Random random) => 
        {
            var direction = dirs[random.Value.NextInt(4)];
            position.Value += direction * speed * deltaTime;
        })
        .Schedule();
    }
}
