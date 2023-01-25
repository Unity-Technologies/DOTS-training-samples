using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
partial struct PlantGrowthSystem : ISystem
{

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float elapsedTime = (float)SystemAPI.Time.ElapsedTime;
        float timeSincePlanted;

        //UnityEngine.Debug.Log("Trying to grow a plant");
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var plant in SystemAPI.Query<PlantAspect>())
        {
            //UnityEngine.Debug.Log("PREgrowing a plant " + plant.ReadyToPick + " and " + plant.PickedAndHeld);
            if (plant.ReadyToPick || plant.PickedAndHeld)
            {
                continue;
            }

            //plant is not fully grown

            timeSincePlanted = elapsedTime - plant.Plant.ValueRW.timePlanted;
            float scale = timeSincePlanted / plant.Plant.ValueRW.timeToGrow;
            scale = math.clamp(scale, 0, 1);
            plant.Transform.LocalScale = scale;

            if(scale >= 1) //plant is now grown
            {
                //state.EntityManager.AddComponent<PlantFinishedGrowing>(plant.Self);
                ecb.AddComponent<PlantFinishedGrowing>(plant.Self);
                plant.ReadyToPick = true;
            }
        }
    }
}