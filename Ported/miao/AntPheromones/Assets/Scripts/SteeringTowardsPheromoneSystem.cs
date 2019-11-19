using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace AntPheromones_ECS
{
    //            EntityQuery qquery = GetEntityQuery(ComponentType.ReadOnly<MapWidth>());
//            Entity e = qquery.GetSingletonEntity();
//            EntityManager.GetSharedComponentData<MapWidth>(e);
//            var something= this.GetSingleton<MapWidth>();

    public class SteeringTowardsPheromoneSystem : JobComponentSystem
    {
//        public BlobAssetReference<ObstacleBlobs> Obstacles { get; private set; }

        protected override void OnCreate()
        {
            this.PheromoneColours = AntPheromones_ECS.PheromoneColours.Generate();
//            this.Obstacles = ObstacleBlobs.Generate();
        }

        public BlobAssetReference<PheromoneColours> PheromoneColours { get; private set; }

        private struct Job : IJobForEach<Position, FacingAngle>
        {
            public BlobArray<Color> PheromoneColours;

//            public float SteerAmount { get; private set; }

            public void Execute(ref Position position, ref FacingAngle facingAngle)
            {
                const float SteeringStrength = 0.015f;
                const float Distance = 3;

                float result = 0;

                for (int i = -1; i <= 1; i += 2)
                {
                    float angle = facingAngle.Value + i * Mathf.PI * 0.25f;
                    int candidateDestinationX = (int) (position.Value.x + Mathf.Cos(angle) * Distance);
                    int candidateDestinationY = (int) (position.Value.y + Mathf.Sin(angle) * Distance);

                    if (Map.IsWithinBounds(candidateDestinationX, candidateDestinationY))
                    {
                        int pheromoneIndex = candidateDestinationX + candidateDestinationY * Map.Width;
                        float redValue = this.PheromoneColours[pheromoneIndex].r;
                        result += redValue * i;
                    }
                }

//                this.SteerAmount = Mathf.Sign(result);
                facingAngle.Value += Mathf.Sign(result) * SteeringStrength;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            return new Job {PheromoneColours = this.PheromoneColours.Value.Colours}.Schedule(this, inputDependencies);
        }
    }
}
