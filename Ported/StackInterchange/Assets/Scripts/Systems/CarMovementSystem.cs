using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class CarMovementSystem : SystemBase
{
    private EntityCommandBufferSystem ecbSystem;

    protected override void OnCreate()
    {
        ecbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.ForEach((Entity carEntity, int entityInQueryIndex, ref Translation trans, ref Rotation rotation, ref CarMovement movement) =>
        {
            var pos = trans.Value;
            trans.Value = math.lerp(trans.Value, movement.NextNode.Value, movement.Velocity);

            var dist = math.distance(trans.Value, movement.NextNode.Value);

            if (dist < 1f)
            {
                // TODO: set node's next node
                movement.NextNode = new Translation{Value = new float3(10,0,10)};
                rotation.Value = quaternion.LookRotation(movement.NextNode.Value, new float3(0,0,1));

                // Destroy if no next node
                // ecb.DestroyEntity(entityInQueryIndex, carEntity);
            }

        }).ScheduleParallel();

        ecbSystem.AddJobHandleForProducer(Dependency);
    }

}