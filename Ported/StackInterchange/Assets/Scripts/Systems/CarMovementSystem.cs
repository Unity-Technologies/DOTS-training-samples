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
            // temp
            var goalPos = GetComponent<LocalToWorld>(movement.NextNode).Position;

            // Check distance
            var dist = math.distance(trans.Value, goalPos);

            if (dist < 1f)
            {
                // Destroy if no next node
                var testComp = GetComponentDataFromEntity<RoadNode>(true);
                var tmpNode = GetComponent<RoadNode>(movement.NextNode).nextNode;
                if (testComp.Exists(tmpNode)) { 
                    movement.NextNode = GetComponent<RoadNode>(movement.NextNode).nextNode;
                    rotation.Value = quaternion.LookRotation(goalPos, new float3(0,0,1));
                }
                else {
                    ecb.DestroyEntity(entityInQueryIndex, carEntity);   
                }
            }

            // Move towards next node
            trans.Value = math.lerp(trans.Value, goalPos, movement.Velocity);

        }).Schedule();

        ecbSystem.AddJobHandleForProducer(Dependency);
    }

}