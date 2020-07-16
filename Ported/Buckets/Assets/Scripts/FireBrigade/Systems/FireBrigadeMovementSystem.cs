using System;
using FireBrigade.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Water;

namespace FireBrigade.Systems
{
    public class FireBrigadeMovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            
            // Calculate formation for chain
            Entities.ForEach(
                (Entity entity,
                    ref GoalPosition goalPosition,
                    in WaterTarget waterPosition,
                    in FireTarget firePosition,
                    in GroupIdentifier groupID,
                    in GroupCount numFighters,
                    in RoleIndex roleIndex,
                    in GroupRole role) =>
                {

                    float3 newGoalPosition = new float3();

                    switch (role.Value)
                    {
                        case FirefighterRole.scooper:
                            newGoalPosition = waterPosition.Position;
                            break;
                        case FirefighterRole.thrower:
                            newGoalPosition = firePosition.Position;
                            break;
                        case FirefighterRole.full:
                            newGoalPosition = GetChainPosition(roleIndex.Value, numFighters.Value, waterPosition.Position,
                                firePosition.Position);
                            break;
                        case FirefighterRole.empty:
                            newGoalPosition = GetChainPosition(roleIndex.Value, numFighters.Value, firePosition.Position,
                                waterPosition.Position);
                            break;
                    }

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

        private static float3 GetChainPosition(int _index, int _chainLength, float3 _startPos, float3 _endPos)
        {
            // adds two to pad between the SCOOPER AND THROWER
            float progress = (float) _index / _chainLength;
            float curveOffset = math.sin(progress * math.PI) * 1f;

            // get Vec2 data
            float2 heading = new float2(_startPos.x, _startPos.z) -  new float2(_endPos.x, _endPos.y);
            float distance = math.length(heading);
            float2 direction = heading / distance;
            float2 perpendicular = new float2(direction.y, -direction.x);

            //Debug.Log("chain progress: " + progress + ",  curveOffset: " + curveOffset);
            return math.lerp(_startPos, _endPos, (float)_index / (float)_chainLength) + (new float3(perpendicular.x, 0f, perpendicular.y) * curveOffset);
        }
    }
}