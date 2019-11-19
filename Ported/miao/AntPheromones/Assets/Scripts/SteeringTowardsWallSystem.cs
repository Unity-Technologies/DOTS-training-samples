using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace AntPheromones_ECS
{
    public class SteeringTowardsWallSystem : JobComponentSystem
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            this.Obstacles = MapObstacles.Generate();
        }

        public BlobAssetReference<MapObstacles> Obstacles { get; private set; }

        private struct Job : IJobForEach<Position, FacingAngle>
        {
            public BlobAssetReference<MapObstacles> Obstacles;
            
//            public float SteerAmount { get; private set; }
            
            public void Execute(ref Position position, ref FacingAngle facingAngle)
            {
                const float Strength = 0.12f;
                const float Distance = 1.5f;
                
                float result = 0;
                 
                for (int i=-1; i<=1; i+=2) 
                {
                    float angle = facingAngle.Value + i * Mathf.PI * 0.25f;
                    int candidateDestinationY = (int)(position.Value.y + Mathf.Sin(angle) * Distance);
                    int candidateDestinationX = (int)(position.Value.x + Mathf.Cos(angle) * Distance);

                    if (!Map.IsWithinBounds(candidateDestinationX, candidateDestinationY))
                    {
                        continue;
                    }

                    if (this.Obstacles.Value.GetObstacleBucket(candidateDestinationX, candidateDestinationY).Length > 0)
                    {
                        result -= i;
                    }
                }

//                this.SteerAmount = Mathf.Sign(result);
                facingAngle.Value += Mathf.Sign(result) * Strength;
            }
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new Job{ Obstacles = this.Obstacles }.Schedule(this, inputDeps);
        }
    }
}