using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace AntPheromones_ECS
{
    [UpdateAfter(typeof(ChangeFacingAngleAccordingToVelocitySystem))]
    public class DropPheromoneSystem : JobComponentSystem
    {
        private EntityQuery _mapQuery;
        private EntityQuery _steeringMovementQuery;
        private (bool IsRetrieved, float MaxSpeed) _steeringMovement;
        private EntityQuery _pheromoneQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            
            this._pheromoneQuery = GetEntityQuery(ComponentType.ReadOnly<PheromoneColourRValueBuffer>());
            this._mapQuery = 
                GetEntityQuery(ComponentType.ReadOnly<Map>());
            this._steeringMovementQuery =
                GetEntityQuery(ComponentType.ReadOnly<SteeringMovement>());
        }
       
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            if (!this._steeringMovement.IsRetrieved)
            {
                this._steeringMovement = 
                    (IsRetrieved: true,
                    MaxSpeed: this._steeringMovementQuery.GetSingleton<SteeringMovement>().MaxSpeed);
            }
            
            Entity pheromoneRValues = this._pheromoneQuery .GetSingletonEntity();
            var pheromoneColourRValues = 
                GetBufferFromEntity<PheromoneColourRValueBuffer>(isReadOnly: true)[pheromoneRValues];

            var map = this._mapQuery.GetSingleton<Map>();
            
            return new Job
            {
                MapWidth = map.Width,
                TrailVisibilityModifier = map.TrailVisibilityModifier,
                
                MaxSpeed = this._steeringMovement.MaxSpeed,
                
                PheromoneColourRValues = pheromoneColourRValues.AsNativeArray()
            }.ScheduleSingle(this, inputDependencies);
        }

        [BurstCompile]
        private struct Job : IJobForEach<Position, Speed, ResourceCarrier>
        {
            public NativeArray<PheromoneColourRValueBuffer> PheromoneColourRValues;
            public float TrailVisibilityModifier;
            public float MaxSpeed;
            public int MapWidth;

            private const float SearchExcitement = 0.3f;
            private const float CarryExcitement = 1f;
            private const float FixedDeltaTime = 1 / 50f;
            
            public void Execute(
                [ReadOnly] ref Position position, 
                [ReadOnly] ref Speed speed,
                [ReadOnly] ref ResourceCarrier resourceCarrier)
            {
                int2 targetPosition = (int2)math.floor(position.Value);

                if (math.any(targetPosition < 0) || math.any(targetPosition >= this.MapWidth))
                {
                    return;
                }

                float excitement = resourceCarrier.IsCarrying ? CarryExcitement : SearchExcitement;
                float strength = excitement * speed.Value / this.MaxSpeed;
                
                int index = targetPosition.x + targetPosition.y * this.MapWidth;
                
                float rValue = this.PheromoneColourRValues[index];
                rValue += this.TrailVisibilityModifier * FixedDeltaTime * strength * (1f - rValue);
                
                this.PheromoneColourRValues[index] = math.min(rValue, 1f);
            }
        }
    }
}