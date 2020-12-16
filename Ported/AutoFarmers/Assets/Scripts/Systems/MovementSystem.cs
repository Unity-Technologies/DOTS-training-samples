using Unity.Entities;
using Unity.Transforms;

public class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        Entities
            .ForEach((Entity entity, ref Translation translation, in Velocity speed) =>
            {
                translation.Value += speed.Value * deltaTime;
            }).Run();
    }
}
