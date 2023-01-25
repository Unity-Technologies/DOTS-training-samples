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
            //silo.Cash += 1;
            if (silo.Cash > silo.FarmerCost && silo.HireType == HireTypes.HIRE_FARMER)
            {
                Entity newFarmer = state.EntityManager.Instantiate(config.FarmerPrefab);
                //TODO Maybe transition this into LocalTransform
                state.EntityManager.SetComponentData<WorldTransform>(newFarmer, silo.Transform.WorldTransform);
                silo.Cash -= silo.FarmerCost;
                silo.FarmersSpawned += 1;
                if (silo.FarmersSpawned >= silo.DronesUnlockLevel)
                    silo.HireType = HireTypes.HIRE_DRONE;
                else
                    silo.HireType = HireTypes.HIRE_FARMER;
            }
            if (silo.Cash > silo.DroneCost && silo.HireType == HireTypes.HIRE_DRONE)
            {
                Entity newDrone = state.EntityManager.Instantiate(config.DronePrefab);
                //TODO Maybe transition this into LocalTransform
                WorldTransform temp = silo.Transform.WorldTransform;
                temp.Position += new Unity.Mathematics.float3(0, 1, 0);
                state.EntityManager.SetComponentData<WorldTransform>(newDrone, temp);
                silo.Cash -= silo.DroneCost;
                silo.HireType = HireTypes.HIRE_FARMER;
                silo.DronesSpawned += 1;
            }
        }
    }
}
