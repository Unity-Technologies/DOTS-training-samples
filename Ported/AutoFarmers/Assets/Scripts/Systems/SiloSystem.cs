using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;

[BurstCompile]
public partial struct SiloSystem : ISystem
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
        var config = SystemAPI.GetSingleton<Config>();
        foreach (var silo in SystemAPI.Query<SiloAspect>())
        {
            silo.Cash += 1;
            if (silo.Cash > silo.FarmerCost)
            {
                Entity newFarmer = state.EntityManager.Instantiate(config.FarmerPrefab);
                //TODO Maybe transition this into LocalTransform
                state.EntityManager.SetComponentData<WorldTransform>(newFarmer, silo.Transform.WorldTransform);
                silo.Cash -= silo.FarmerCost;
            }
        }
    }
}
