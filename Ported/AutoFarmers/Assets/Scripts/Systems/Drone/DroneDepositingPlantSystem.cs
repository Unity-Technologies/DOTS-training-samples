using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct DroneDepositingPlantSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameConfig>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var gameConfig = SystemAPI.GetSingleton<GameConfig>();
        

        var plantsDepositted = 0;
        int2 posDepositted = new int2();
        foreach (var drone in SystemAPI.Query<DroneDepositingPlantAspect>())
        {
            if (drone.AtDesiredLocation)
            {
                ecb.DestroyEntity(drone.Plant);
                ecb.RemoveComponent<DroneDepositPlantIntent>(drone.Self);
                ecb.AddComponent<DroneFindPlantIntent>(drone.Self);
                posDepositted = drone.DesiredLocation;
                plantsDepositted++;
            }
        }

        if (plantsDepositted > 0)
        {
            var moneyEntity = SystemAPI.GetSingletonEntity<FarmMoney>();
            var money = SystemAPI.GetSingletonRW<FarmMoney>();

            money.FarmerMoney += gameConfig.MoneyPerPlant * plantsDepositted;
            money.DroneMoney += gameConfig.MoneyPerPlant * plantsDepositted;
            money.LastDepositLocaiton = posDepositted;
            ecb.SetComponent(moneyEntity, money);
        }

    }
}
