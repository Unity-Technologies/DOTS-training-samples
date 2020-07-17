using System;
using Fire;
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
        struct FireData
        {
            public TemperatureComponent temperatureComponent;
            public LocalToWorld localToWorldComponent;
        }

        protected override void OnUpdate()
        {
            // Check if we have initialized
            EntityQuery queryGroup = GetEntityQuery(typeof(Initialized));
            if (queryGroup.CalculateEntityCount() == 0)
            {
                return;
            }

            float deltaTime = Time.DeltaTime;

            // Calculate formation for chain and update position for last frame's target
            Entities.ForEach(
                (
                    ref Translation translation,
                    in WaterTarget waterPosition,
                    in FireTarget firePosition,
                    in GroupCount numFighters,
                    in RoleIndex roleIndex,
                    in MovementSpeed speed,
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

                    if (math.distancesq(newGoalPosition, translation.Value) < 0.01f) return;

                    var movementVector = newGoalPosition - translation.Value;
                    movementVector = math.normalize(movementVector);
                    translation.Value += movementVector * speed.Value * deltaTime;
                }).ScheduleParallel();


            // Get fire
            var fireBufferEntity = GetSingletonEntity<FireBufferElement>();
            var fireLookup = GetBufferFromEntity<FireBufferElement>(true);
            var fireBuffer = fireLookup[fireBufferEntity];

            // Create cached native array of temperature components to read neighbor temperature data in jobs
            NativeArray<FireData> fireData = new NativeArray<FireData>(fireBuffer.Length, Allocator.TempJob);
            for (int i = 0; i < fireBuffer.Length; i++)
            {
                FireData data = new FireData
                {
                    temperatureComponent = EntityManager.GetComponentData<TemperatureComponent>(fireBuffer[i].FireEntity),
                    localToWorldComponent = EntityManager.GetComponentData<LocalToWorld>(fireBuffer[i].FireEntity)
                };
                fireData[i] = data;
            }

            // Update target fire to always be the one closest to our water
            Entities
                .WithDeallocateOnJobCompletion(fireData)
                .ForEach((ref FireTarget fireTarget, in WaterTarget waterTarget) =>
                {
                    // pick a fire cell closest to our chosen water position that is above a threshold in temp
                    var closestDistance = float.MaxValue;
                    var closestFireIndex = -1;
                    float3 closestPosition = new float3();
                    for (int fireDataIndex = 0; fireDataIndex < fireData.Length; fireDataIndex++)
                    {
                        var data = fireData[fireDataIndex];
                        var firePosition = data.localToWorldComponent.Position;
                        if (data.temperatureComponent.Value < 0.2) continue;
                        
                        var distance = math.distancesq(firePosition, waterTarget.Position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestPosition = firePosition;
                        }
                    }
                    closestPosition.y = 0f;
                    fireTarget.Position = closestPosition;
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