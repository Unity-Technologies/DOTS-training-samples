using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class CarMovementSystem : SystemBase
{
    private EntityCommandBufferSystem ecbSystem;
    private Random random;

    protected override void OnCreate()
    {
        ecbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        random = new Random(0x1234567);
    }

    protected override void OnUpdate()
    {
        var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();
        var tryExit = random.NextBool();

        Entities.ForEach((Entity carEntity, int entityInQueryIndex, ref Translation trans, ref Rotation rotation, ref CarMovement movement) =>
        {
            // temp
            var goalPos = GetComponent<LocalToWorld>(movement.NextNode).Position;

            if (movement.distanceTraveled < .1f)
            {
                movement.distanceToNext = math.distance(trans.Value, goalPos);
                movement.distanceTraveled = 0;
                movement.travelVec = goalPos - trans.Value;
                movement.travelVec = math.normalize(movement.travelVec);
            }
            
            // Check distance
            var dist = math.distance(trans.Value, goalPos);

            if (dist < 1f)
            {
                // Destroy if no next node
                var testComp = GetComponentDataFromEntity<RoadNode>(true);
                var tmpRoadNode = GetComponent<RoadNode>(movement.NextNode);

                bool destroyEntity = tmpRoadNode.nextNode == Entity.Null && tmpRoadNode.exitNode == Entity.Null;
                bool choiceAvailable = tmpRoadNode.nextNode != Entity.Null && tmpRoadNode.exitNode != Entity.Null;

                if (!destroyEntity)
                {
                    var tmpNode = (tmpRoadNode.nextNode != Entity.Null) ? tmpRoadNode.nextNode : tmpRoadNode.exitNode;

                    if (choiceAvailable && tryExit)
                    {
                        tmpNode = tmpRoadNode.exitNode;
                    }
                    
                    
                    movement.NextNode = tmpNode;

                    // ???
                    float tmp = math.abs(math.mul(goalPos.x, goalPos.z)) % 360;
                    if (tmp > 90 && tmp < 180){
                        rotation.Value = math.mul(rotation.Value, quaternion.RotateY(math.mul(goalPos.x, goalPos.z)));                        
                    }
                    movement.distanceTraveled = 0;
                }
                else{
                    ecb.DestroyEntity(entityInQueryIndex, carEntity);   
                }
            }

            // Move towards next node
            movement.distanceTraveled += movement.Velocity;
            trans.Value += movement.travelVec * movement.Velocity;

        }).ScheduleParallel();

        ecbSystem.AddJobHandleForProducer(Dependency);
    }

}
