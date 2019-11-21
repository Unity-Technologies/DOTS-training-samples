using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace AntPheromones_ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(RandomizeFacingAngleSystem))]
    public class CalculateWallSteeringSystem : JobComponentSystem
    {
        private MapComponent _map;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            this._map = GetEntityQuery(ComponentType.ReadOnly<MapComponent>()).GetSingleton<MapComponent>();
        }

        private struct Job : IJobForEach<PositionComponent, FacingAngleComponent>
        {
            public float MapWidth;
            public BlobAssetReference<ObstacleData> Obstacles;
            
            public void Execute(ref PositionComponent position, ref FacingAngleComponent facingAngleComponent)
            {
                const float Strength = 0.12f;
                const float Distance = 1.5f;
                
                float result = 0;

                for (int i = -1; i <= 1; i += 2) 
                {
                    float angle = facingAngleComponent.Value + i * math.PI * 0.25f;
                   
                    float candidateDestinationY = position.Value.y + math.sin(angle) * Distance;
                    float candidateDestinationX = position.Value.x + math.cos(angle) * Distance;

                    if (candidateDestinationX < 0 || candidateDestinationY < 0 ||
                        candidateDestinationX >= this.MapWidth || candidateDestinationY >= this.MapWidth)
                    {
                        continue;
                    }

                    if (this.Obstacles.Value.HasObstacle(new float2(candidateDestinationX, candidateDestinationY)))
                    {
                        result -= i;
                    }
                }
                facingAngleComponent.Value += math.sign(result) * Strength;
            }
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new Job
            {
                Obstacles = this._map.Obstacles,
                MapWidth = this._map.Width
            }.Schedule(this, inputDeps);
        }
    }
}