using System;
using System.Linq;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

namespace AntPheromones_ECS
{
    //            EntityQuery qquery = GetEntityQuery(ComponentType.ReadOnly<MapWidth>());
//            Entity e = qquery.GetSingletonEntity();
//            EntityManager.GetSharedComponentData<MapWidth>(e);
//            var something= this.GetSingleton<MapWidth>();

    public class SteeringSystem : JobComponentSystem
    {
        public BlobAssetReference<ObstacleBlobs> Obstacles { get; private set; }
            
        protected override void OnCreate()
        {
            this.Obstacles = ObstacleBlobs.Generate();
        }

        private struct SteerTowardsPheromoneJob : IJobForEach<Position, Movement>
        {
            public float SteeringStrength; 
            public float Distance;
            
            public float SteerAmount { get; private set; }

            public void Execute(ref Position position, ref Movement movement)
            {
                float result = 0;

                for (int i = -1; i <= 1; i += 2)
                {
                    float angle = movement.FacingAngle + i * Mathf.PI * 0.25f;
                    int candidateDestinationX = (int)(position.Value.x + Mathf.Cos(angle) * Distance);
                    int candidateDestinationY = (int)(position.Value.y + Mathf.Sin(angle) * Distance);

                    if (Map.IsWithinBounds(candidateDestinationX, candidateDestinationY))
                    {
                        int pheromoneIndex = candidateDestinationX + candidateDestinationY * Map.Width;
                        float redValue = AntManager.Instance.pheromoneColours[pheromoneIndex].r;
                       result += redValue * i;
                    }
                }

                this.SteerAmount = Mathf.Sign(result);
                movement.FacingAngle += this.SteerAmount * this.SteeringStrength;
            }
        }

        private struct SteerTowardsWallJob : IJobForEach<Position, Movement>
        {
            public float SteeringStrength; // 0.12f
            public float Distance; // 1.5f
            public BlobAssetReference<ObstacleBlobs> Obstacles;
            
            public float SteerAmount { get; private set; }
            
            public void Execute(ref Position position, ref Movement movement)
            {
                float result = 0;
                 
                for (int i=-1; i<=1; i+=2) 
                {
                    float angle = movement.FacingAngle + i * Mathf.PI * 0.25f;
                    int candidateDestinationY = (int)(position.Value.y + Mathf.Sin(angle) * this.Distance);
                    int candidateDestinationX = (int)(position.Value.x + Mathf.Cos(angle) * this.Distance);

                    if (!Map.IsWithinBounds(candidateDestinationX, candidateDestinationY))
                    {
                        continue;
                    }
//
//                    if (this.Obstacles.Value.GetObstacleBucket(candidateDestinationX, candidateDestinationY).Length > 0)
//                    {
//                        result -= i;
//                    }
                }

                this.SteerAmount = Mathf.Sign(result);
                movement.FacingAngle += this.SteerAmount * this.SteeringStrength;
            }
        }

        private struct AdjustSpeedJob : IJobForEach<Position, Movement>
        {
            private const float Acceleration = 0.07f;
            
            public float PheromoneSteeringAmount;
            public float WallSteeringAmount;
            
            public void Execute(ref Position position, ref Movement movement)
            {
                var targetSpeed = movement.Speed * 1f - (Mathf.Abs(this.PheromoneSteeringAmount) + Math.Abs(this.WallSteeringAmount)) / 3f;
                movement.Speed += (targetSpeed - movement.Speed) * Acceleration;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var pheromoneJob = new SteerTowardsPheromoneJob
            {
                Distance = 3f,
                SteeringStrength = 0.015f
            };
            JobHandle pheromoneJobHandle = pheromoneJob.Schedule(this, inputDependencies);

            var wallJob = new SteerTowardsWallJob
            { 
                Distance = 1.5f,
                SteeringStrength = 0.12f,
                Obstacles = this.Obstacles
            };
            JobHandle wallJobHandle = wallJob.Schedule(this, pheromoneJobHandle);

            var speedJob = new AdjustSpeedJob
            {
                PheromoneSteeringAmount = pheromoneJob.SteerAmount,
                WallSteeringAmount = wallJob.SteerAmount
            };
            return speedJob.Schedule(this, wallJobHandle);
        }
    }
}
