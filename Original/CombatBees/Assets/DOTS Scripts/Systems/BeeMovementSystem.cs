using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateAfter(typeof(FoodMovementSystem))]
public class BeeMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Entities.ForEach is a job generator, the lambda it contains will be turned
        // into a proper IJob by IL post processing.
        Entities
            .WithName("BeeMovementToTarget")
            .WithAll<BeeTag>()
            .ForEach((Entity e, int entityInQueryIndex, ref PhysicsData physicsData, in Translation translation, in TargetPosition t, in MoveSpeed speed) =>
            {
                var directionVector = t.Value - translation.Value;
                var destVector = math.normalize(directionVector);
                physicsData.a += destVector * speed.Value;
            }).Run();
    }
}