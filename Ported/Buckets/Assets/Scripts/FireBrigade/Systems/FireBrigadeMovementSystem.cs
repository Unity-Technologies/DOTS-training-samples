using FireBrigade.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace FireBrigade.Systems
{
    public class FireBrigadeMovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            
            // Calculate goal
            Entities.ForEach(
                (Entity entity,
                    ref GoalPosition goalPosition,
                    in WaterPosition waterPosition,
                    in FirePosition firePosition,
                    in GroupIdentifier groupID,
                    in GroupCount numFighters,
                    in GroupIndex groupIndex) =>
                {

                    float progress = (float) groupIndex.Value / numFighters.Value;
                    float curveOffset = math.sin(progress * math.PI) * 1f;

                    float2 heading = new float2(waterPosition.Value.x, waterPosition.Value.z) -  new float2(firePosition.Value.x, firePosition.Value.y);
                    float distance = math.length(heading);
                    float2 direction = heading / distance;
                    float2 perpendicular = new float2(direction.y, -direction.x);

                    float3 newGoalPosition;
                    
                    if (groupIndex.Value == 0) newGoalPosition = firePosition.Value;
                    else if (groupIndex.Value == numFighters.Value - 1) newGoalPosition = waterPosition.Value;
                    else
                    {
                        if (groupIndex.Value % 2 == 0)
                        {
                            newGoalPosition = math.lerp(firePosition.Value, waterPosition.Value,
                                (float) groupIndex.Value / (float) numFighters.Value +
                                new float3(perpendicular.x, 0f, perpendicular.y) * curveOffset);
                        }
                        else
                        {
                            newGoalPosition = math.lerp(waterPosition.Value, firePosition.Value,
                                (float) groupIndex.Value / (float) numFighters.Value +
                                new float3(perpendicular.x, 0f, perpendicular.y) * curveOffset);
                        }
                    }
                        
                        
                    
                    
                    // var distanceBetweenTargets = math.distance(waterPosition.Value, firePosition.Value);
                    // var spacing = distanceBetweenTargets / numFighters.Value;
                    // var towardsFireVector = firePosition.Value - waterPosition.Value;
                    // towardsFireVector = math.normalize(towardsFireVector);
                    //
                    // var newGoalPosition = new GoalPosition();
                    // if (groupIndex.Value == 0) newGoalPosition.Value = firePosition.Value;
                    // else if (groupIndex.Value == numFighters.Value - 1) newGoalPosition.Value = waterPosition.Value;
                    // else
                    // {
                    //     newGoalPosition.Value = waterPosition.Value + (towardsFireVector * groupIndex.Value);
                    // }

                    goalPosition.Value = newGoalPosition;

                }).Schedule();
            
            // Move toward goal
            Entities.ForEach(
                (ref Translation translation,
                in GoalPosition goalPosition,
                in MovementSpeed speed) =>
            {
                if (math.distance(goalPosition.Value, translation.Value) < 0.1f) return;

                var movementVector = goalPosition.Value - translation.Value;
                movementVector = math.normalize(movementVector);
                translation.Value += movementVector * speed.Value * deltaTime;
                
            }).ScheduleParallel();
        }
    }
}