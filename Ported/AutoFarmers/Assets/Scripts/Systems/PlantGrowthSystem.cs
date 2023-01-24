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

        foreach (var (transform, plant) in SystemAPI.Query<TransformAspect, RefRW<Plant>>().WithAll<Plant>())
        {
            //if (plant.ValueRW.isReadyToPick || plant.ValueRW.pickedAndHeld)
            //{
            //    continue;
            //}

            //plant is not fully grown

            timeSincePlanted = elapsedTime - plant.ValueRW.timePlanted;
            float scale = timeSincePlanted / plant.ValueRW.timeToGrow;
            scale = math.clamp(scale, 0, 1);
            transform.WorldScale = scale;
        }


    }
}