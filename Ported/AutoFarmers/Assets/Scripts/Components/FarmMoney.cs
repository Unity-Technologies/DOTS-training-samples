using Unity.Entities;
using Unity.Mathematics;

public struct FarmMoney : IComponentData
{
    public int FarmerMoney;

    public int DroneMoney;

    public int SpawnedFarmers;

    public int SpawnedDrones;

    public int2 LastDepositLocaiton;
}
