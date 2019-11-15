using System;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace AntPheromones_ECS
{
//    public static class ObstacleBuckets
//    {   
//        private static readonly NativeArray<Obstacle> Empty = new NativeArray<Obstacle>(length: 0, Allocator.None);
//        
//        public static NativeArray<Obstacle> Get(float positionX, float positionY, float mapWidth, int bucketResolution)
//        {
//            int x = (int) (positionX / mapWidth * bucketResolution);
//            int y = (int) (positionY / mapWidth * bucketResolution);
//
//            if (x < 0 || y < 0 || x >= bucketResolution || y >= bucketResolution)
//            {
//                return Empty;
//            }
//
//            return Empty;
//        }
//    }
    
    public struct MapWidth : IComponentData
    {
        public float Value;
    }

    public class ObstacleGenerationSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            throw new System.NotImplementedException();
        }
    }
    
    [UpdateAfter(typeof(ObstacleGenerationSystem))]
    public class SteeringSystem : JobComponentSystem
    {
        private ObstacleGenerationSystem _obstacleGenerationSystem;
        
        protected override void OnCreate()
        {
            this._obstacleGenerationSystem = World.Active.GetOrCreateSystem<ObstacleGenerationSystem>();
        }

        private struct SteerTowardsPheromoneJob  : IJobForEach<Position, Movement>
        {
            public float SteeringStrength; // 0.015f
            public float Distance; // 3f
            public int MapWidth; // 128
            
            public void Execute(ref Position position, ref Movement movement)
            {
                float result = 0;
                 
                for (int i=-1;i<=1;i+=2) 
                {
                    float angle = movement.FacingAngle + i * Mathf.PI * 0.25f;
                    float candidateDestinationX = position.Value.x + Mathf.Cos(angle) * Distance;
                    float candidateDestinationY = position.Value.y + Mathf.Sin(angle) * Distance;
                    
                    bool targetPositionWithinBounds = 
                        candidateDestinationX >= 0 && candidateDestinationY >= 0 && candidateDestinationX < MapWidth && candidateDestinationY < MapWidth;
                    
                    if (targetPositionWithinBounds)
                    {
                        int pheromoneIndex = (int) candidateDestinationX + (int) candidateDestinationY * MapWidth;
                        float redValue = AntManager.Instance.PheromoneColours[pheromoneIndex].r;
                        
                        result += redValue * i; 
                    }
                }

                movement.FacingAngle += result * this.SteeringStrength;
            }
        }
        
        private struct SteerTowardsWallJob : IJobForEach<Position, Movement>
        {
            public float SteeringStrength; // 0.12f
            public float Distance; // 1.5f
            public int BucketResolution; // 64
            public int MapWidth; // 128
            
            public void Execute(ref Position position, ref Movement movement)
            {
                float result = 0;
                 
                for (int i=-1; i<=1; i+=2) 
                {
                    float angle = movement.FacingAngle + i * Mathf.PI * 0.25f;
                    float candidateDestinationX = position.Value.x + Mathf.Cos(angle) * this.Distance;
                    float candidateDestinationY = position.Value.y + Mathf.Sin(angle) * this.Distance;
                    
                    bool targetPositionWithinBounds = 
                        candidateDestinationX >= 0
                        && candidateDestinationY >= 0 
                        && candidateDestinationX < this.MapWidth
                        && candidateDestinationY < this.MapWidth;
                    
                    if (targetPositionWithinBounds)
                    {
                        int obstacleBucketSize = GetObstacleBucket(candidateDestinationX, candidateDestinationY).Length;
                        if (obstacleBucketSize > 0)
                        {
                            result -= i;
                        }
                    }
                }

                movement.FacingAngle += result * this.SteeringStrength;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputsDependencies)
        {
            //            EntityQuery qquery = GetEntityQuery(ComponentType.ReadOnly<MapWidth>());
//            Entity e = qquery.GetSingletonEntity();
//            EntityManager.GetSharedComponentData<MapWidth>(e);
            var something= this.GetSingleton<MapWidth>();
            
            throw new NotImplementedException();
        }
    }
}