using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    public partial struct ObstacleGenerationSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Obstacle>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            var rand = Random.CreateFromIndex(1);

            foreach (RefRW<Obstacle> spawner in SystemAPI.Query<RefRW<Obstacle>>())
            {
                for (int i = 0; i < 10; ++i)
                {
                    Entity newEntity = state.EntityManager.Instantiate(spawner.ValueRO.prefab);
                    state.EntityManager.SetComponentData(newEntity,
                        LocalTransform.FromPosition(rand.NextFloat3()));
                }
            }
        }

        public void rippedCode()
        {
            /*List<Obstacle> output = new List<Obstacle>();
            for (int i=1;i<=obstacleRingCount;i++) {
                float ringRadius = (i / (obstacleRingCount+1f)) * (mapSize * .5f);
                float circumference = ringRadius * 2f * Mathf.PI;
                int maxCount = Mathf.CeilToInt(circumference / (2f * obstacleRadius) * 2f);
                int offset = Random.Range(0,maxCount);
                int holeCount = Random.Range(1,3);
                for (int j=0;j<maxCount;j++) {
                    float t = (float)j / maxCount;
                    if ((t * holeCount)%1f < obstaclesPerRing) {
                        float angle = (j + offset) / (float)maxCount * (2f * Mathf.PI);
                        Obstacle obstacle = new Obstacle();
                        obstacle.position = new Vector2(mapSize * .5f + Mathf.Cos(angle) * ringRadius,mapSize * .5f + Mathf.Sin(angle) * ringRadius);
                        obstacle.radius = obstacleRadius;
                        output.Add(obstacle);
                        //Debug.DrawRay(obstacle.position / mapSize,-Vector3.forward * .05f,Color.green,10000f);
                    }
                }
            }*/
        }
    }
}