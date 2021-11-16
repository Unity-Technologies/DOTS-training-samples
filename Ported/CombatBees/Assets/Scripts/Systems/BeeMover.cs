using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class BeeMover : SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.DeltaTime;
        float3 gravityVector = new float3(0, -2, 0);

        Entities
            .WithAll<Gravity>()
            .ForEach((ref Velocity velocity, in Gravity gravity) =>
        {
            velocity.Value += gravityVector * time;
        }).Run();

        Entities
            .WithAll<Gravity>()
            .ForEach((ref Translation translation, in Velocity velocity) => 
        {
            translation.Value = translation.Value + velocity.Value * time;
        }).Schedule();

        Entities
            .WithStructuralChanges()
            .WithAll<Gravity>()
            .WithNone<Decay>()
            .ForEach((Entity entity, ref Translation translation) =>
        {
            if (translation.Value.y < -5)
            {
                translation.Value.y = -5;
                EntityManager.RemoveComponent<Gravity>(entity);
                EntityManager.AddComponentData<Decay>(entity, new Decay { Rate = 1.0f });
            }
        }).Run();
    }
}
