using Metro;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct DoorSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        //transformTypeHandle = state.GetComponentTypeHandle<LocalTransform>();
        // This makes the system not update unless at least one entity exists that has the Door component.
        state.RequireForUpdate<Door>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var em = state.EntityManager;

        foreach (var (transform, door, entity) in
                 SystemAPI.Query<RefRW<LocalTransform>, RefRW<Door>>()
                     .WithEntityAccess())
        {
            if (em.IsComponentEnabled<UnloadingComponent>(entity))
            {
                door.ValueRW.Timer += SystemAPI.Time.DeltaTime;

                var lerp = math.min(1f, door.ValueRW.Timer / Door.OpeningTime);
                transform.ValueRW.Position = math.lerp(door.ValueRO.ClosedPosition, door.ValueRO.OpenPosition, lerp);

                if (lerp >= 1f)
                {
                    door.ValueRW.Timer = 0f;
                    em.SetComponentEnabled<UnloadingComponent>(entity, false);
                }
            }

            if (em.IsComponentEnabled<DepartingComponent>(entity))
            {
                door.ValueRW.Timer += SystemAPI.Time.DeltaTime;

                var lerp = math.min(1f, door.ValueRW.Timer / Door.OpeningTime);
                transform.ValueRW.Position = math.lerp(door.ValueRO.OpenPosition, door.ValueRO.ClosedPosition, lerp);

                if (lerp >= 1f)
                {
                    door.ValueRW.Timer = 0f;
                    em.SetComponentEnabled<DepartingComponent>(entity, false);
                }
            }
        }
    }
}
