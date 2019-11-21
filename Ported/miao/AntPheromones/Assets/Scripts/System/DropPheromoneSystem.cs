using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace AntPheromones_ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(CollideWithObstacleSystem))]
    public class DropPheromoneSystem : JobComponentSystem
    {
        private MapComponent _map;
        private SteeringMovementComponent _steeringMovement;
        private DynamicBuffer<PheromoneColourRValue> _pheromoneColourRValues;

        private struct Job : IJobForEach<PositionComponent, SpeedComponent, ResourceCarrierComponent>
        {
            public DynamicBuffer<PheromoneColourRValue> PheromoneColourRValues;
            public float TrailVisibilityModifier;
            public float MaxSpeed;
            public int MapWidth;

            public void Execute(
                [ReadOnly] ref PositionComponent position, 
                [ReadOnly] ref SpeedComponent speed,
                [ReadOnly] ref ResourceCarrierComponent resourceCarrier)
            {
                const float CarrierExcitement = 1f;
                const float SearcherExcitement = 0.3f;

                int2 targetPosition = (int2)math.floor(position.Value);

                if (math.any(targetPosition < 0) || math.any(targetPosition >= this.MapWidth))
                {
                    return;
                }

                float excitement = resourceCarrier.IsCarrying ? CarrierExcitement : SearcherExcitement;
                float strength = excitement * speed.Value / this.MaxSpeed;
                
                int index = targetPosition.x + targetPosition.y * this.MapWidth;

                float rValue = this.PheromoneColourRValues[index];
                rValue +=
                    math.min(
                        this.TrailVisibilityModifier * strength * Time.fixedDeltaTime * (1f - this.PheromoneColourRValues[index]), 1f);
                
                this.PheromoneColourRValues[index] = rValue;
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            
            this._map = 
                GetEntityQuery(ComponentType.ReadOnly<MapComponent>()).GetSingleton<MapComponent>();
            this._steeringMovement =
                GetEntityQuery(ComponentType.ReadOnly<SteeringMovementComponent>()).GetSingleton<SteeringMovementComponent>();
            
            Entity pheromoneRValues = GetEntityQuery(ComponentType.ReadOnly<PheromoneColourRValue>()).GetSingletonEntity();
            this._pheromoneColourRValues = GetBufferFromEntity<PheromoneColourRValue>(isReadOnly: true)[pheromoneRValues];
        }
       
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            return new Job
            {
                MapWidth = this._map.Width,
                MaxSpeed = this._steeringMovement.MaxSpeed,
                TrailVisibilityModifier = this._map.TrailVisibilityModifier,
                PheromoneColourRValues = this._pheromoneColourRValues
            }.Schedule(this, inputDependencies);
        }
    }
}