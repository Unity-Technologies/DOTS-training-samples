using Unity.Entities;
using Unity.Transforms;

partial struct ApplyGravityJob : IJobEntity
{
    public float Floor; 
    public float Gravity;
    public float DeltaTime;

    void Execute(Entity e, ref TransformAspect t, ref Velocity v)
    {
        if (t.LocalPosition.y - Floor > float.Epsilon)
        {
            v.Value.y += Gravity * DeltaTime;
        }
        else
        {
            v.Value.y = 0f;

            // apply ground friction for bonus points?
        }
    }
}

[UpdateBefore(typeof(BeeClampingSystem))]
partial struct BallisticMovementSystem : ISystem
{
    private EntityQuery m_ballisticEntities;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FieldConfig>();

        m_ballisticEntities = state.GetEntityQuery(ComponentType.ReadOnly<Decay>(), ComponentType.ReadWrite<Velocity>());
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var fieldConfig = SystemAPI.GetSingleton<FieldConfig>();

        new ApplyGravityJob { Floor = -fieldConfig.FieldScale.y * .5f, Gravity = fieldConfig.FieldGravity, DeltaTime = state.Time.DeltaTime }
            .ScheduleParallel(m_ballisticEntities);
    }
}