using System;
using Unity.Collections;
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
        public const int MapWidth = 128;
        public const int BucketResolution = 64;

        [ReadOnly] private NativeArray<Obstacle> Empty = new NativeArray<Obstacle>(length: 0, Allocator.None)
            
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            throw new System.NotImplementedException();
        }

        public void GetObstacleBucket(float candidateDestinationX, float candidateDestinationY)
        {
            int x = (int) (candidateDestinationX / MapWidth * BucketResolution);
            int y = (int) (candidateDestinationY / MapWidth * BucketResolution);

            if (IsWithinBounds(x, y))
            {
                return 
            }
            
        }

        public static bool IsWithinBounds(int positionX, int positionY)
        {
            return positionX >= 0 && positionY >= 0 && positionX < MapWidth && positionY < MapWidth;
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
            public ObstacleGenerationSystem ObstacleGenerationSystem;
            
            public void Execute(ref Position position, ref Movement movement)
            {
                float result = 0;
                 
                for (int i=-1;i<=1;i+=2) 
                {
                    float angle = movement.FacingAngle + i * Mathf.PI * 0.25f;
                    int candidateDestinationX = (int)(position.Value.x + Mathf.Cos(angle) * Distance);
                    int candidateDestinationY = (int)(position.Value.y + Mathf.Sin(angle) * Distance);

                    if (ObstacleGenerationSystem.IsWithinBounds(candidateDestinationX, candidateDestinationY))
                    {
                        int pheromoneIndex = candidateDestinationX + candidateDestinationY * ObstacleGenerationSystem.MapWidth;
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
            public ObstacleGenerationSystem ObstacleGenerationSystem;
            
            public void Execute(ref Position position, ref Movement movement)
            {
                float result = 0;
                 
                for (int i=-1; i<=1; i+=2) 
                {
                    float angle = movement.FacingAngle + i * Mathf.PI * 0.25f;
                    int candidateDestinationY = (int)(position.Value.y + Mathf.Sin(angle) * this.Distance);
                    int candidateDestinationX = (int)(position.Value.x + Mathf.Cos(angle) * this.Distance);
                    
                    if (ObstacleGenerationSystem.IsWithinBounds(candidateDestinationX, candidateDestinationY))
                    {
                        int obstacleBucketSize = this.ObstacleGenerationSystem.GetObstacleBucket(candidateDestinationX, candidateDestinationY).Length;
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